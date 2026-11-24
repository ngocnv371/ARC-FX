"""Synthesises ARC FX's own sounds into audio/<name>.wav (mono, 16-bit, 44.1 kHz).

Built from noise, sines and filters with numpy and scipy only; the same seed always gives the same
file. Many recipes are ARC Dungeons' (D:\\dev\\ARC-Dungeons\\tools\\gen_sfx.py), the rest are made
here. The .ogg sounds beside them are CC0 recordings and Rogue Guys' sounds (audio/CREDITS.md).
FxSounds.cs says which effect plays which.

    python tools/gen_sfx.py                  # every sound
    python tools/gen_sfx.py boom hit_fire    # just these

Needs numpy and scipy.
"""

import sys
import wave
from pathlib import Path

import numpy as np
from scipy import signal

OUT = Path(__file__).resolve().parent.parent / "audio"
SR = 44100
rng = np.random.default_rng(7)


# ---- building blocks ----------------------------------------------------------------------------

def t(d):
    return np.arange(int(d * SR)) / SR


def noise(d):
    return rng.standard_normal(int(d * SR))


def env(d, attack, decay, hold=0.0):
    """Rises linearly over `attack` seconds, holds, then falls exponentially (`decay` = time to 1/e)."""
    x = t(d)
    return np.where(x < attack, x / max(attack, 1e-4), np.exp(-np.maximum(x - attack - hold, 0) / decay))


def bell(d, peak, width):
    """A smooth swell centred at `peak` seconds."""
    x = t(d)
    return np.exp(-0.5 * ((x - peak) / width) ** 2)


def sos(kind, f, order=2):
    f = np.clip(np.atleast_1d(f), 20, SR / 2 - 100)
    return signal.butter(order, f if len(f) > 1 else f[0], btype=kind, fs=SR, output="sos")


def lp(x, f, order=2):
    return signal.sosfilt(sos("lowpass", f, order), x)


def hp(x, f, order=2):
    return signal.sosfilt(sos("highpass", f, order), x)


def bp(x, lo, hi, order=2):
    return signal.sosfilt(sos("bandpass", [lo, hi], order), x)


def swept(x, f0, f1, kind="lowpass", block=256):
    """A filter whose cutoff glides from f0 to f1 (exponentially) across the sound."""
    out = np.zeros_like(x)
    zi = None
    n = len(x)
    for start in range(0, n, block):
        u = start / max(1, n - 1)
        s = sos(kind, f0 * (f1 / f0) ** u)
        if zi is None:
            zi = np.zeros((s.shape[0], 2))
        out[start:start + block], zi = signal.sosfilt(s, x[start:start + block], zi=zi)
    return out


def glide(d, f0, f1, curve=1.0):
    """A sine gliding from f0 to f1 Hz."""
    x = t(d)
    f = f0 * (f1 / f0) ** ((x / d) ** curve)
    return np.sin(2 * np.pi * np.cumsum(f) / SR)


def tone(d, f, decay, attack=0.002, start=0.0, detune=0.0):
    """A plucked or struck sine (a bell partial) starting at `start`."""
    x = t(d)
    y = np.zeros_like(x)
    on = x >= start
    xs = x[on] - start
    y[on] = np.sin(2 * np.pi * f * (1 + detune) * xs) * np.minimum(xs / attack, 1) * np.exp(-xs / decay)
    return y


def crackle(d, rate, lo, hi, decay=0.004, fade=None):
    """Sparse random clicks (fire, rubble, sparks), band-passed."""
    n = int(d * SR)
    y = np.zeros(n)
    for at in rng.integers(0, n, int(rate * d)):
        k = np.arange(min(int(SR * decay * 6), n - at))
        y[at:at + len(k)] += rng.choice([-1, 1]) * rng.uniform(0.3, 1.0) * np.exp(-k / (SR * decay))
    y = bp(y, lo, hi)
    if fade:
        y *= np.exp(-t(d) / fade)
    return y


def bubbles(d, count, lo=250, hi=700, span=None):
    y = np.zeros(int(d * SR))
    span = span or d * 0.7
    for _ in range(count):
        start = rng.uniform(0, span)
        f0 = rng.uniform(lo, hi)
        length = rng.uniform(0.02, 0.05)
        k = t(length)
        f = f0 * (1 + 1.2 * k / length)
        b = np.sin(2 * np.pi * np.cumsum(f) / SR) * np.sin(np.pi * k / length)
        at = int(start * SR)
        y[at:at + len(b)] += b[: len(y) - at] * rng.uniform(0.4, 1)
    return y


def room(x, mix=0.18, taps=((0.031, 0.5), (0.047, 0.4), (0.071, 0.3), (0.113, 0.2))):
    """A little stone-room ambience: a few dark, decaying echoes."""
    y = np.concatenate([x, np.zeros(int(0.2 * SR))])
    wet = np.zeros_like(y)
    for delay, gain in taps:
        k = int(delay * SR)
        wet[k:] += gain * y[:-k]
    return y + mix * lp(wet, 3000)


def finish(x, peak=0.9):
    """Removes any DC, fades the last 10 ms out and sets the peak."""
    x = hp(x, 30)
    fade = int(0.01 * SR)
    x[-fade:] *= np.linspace(1, 0, fade)
    m = np.max(np.abs(x))
    return x / m * peak if m > 0 else x


def whoosh(d=0.38, lo=350, hi=2600, peak=0.17):
    n = noise(d)
    y = swept(n, lo, hi, "lowpass") * bell(d, peak, d / 5)
    y += 0.4 * hp(n, 2500) * bell(d, peak, d / 8)
    return room(y, 0.08)


# ---- ARC Dungeons' recipes ----------------------------------------------------------------------

def whirl():
    d = 0.6
    y = whoosh(d, 400, 2200, 0.3)[: int(d * SR)]
    return y * (0.65 + 0.35 * np.sin(2 * np.pi * 14 * t(d)))


def throw():
    d = 0.42
    y = whoosh(d, 500, 3000, 0.16)[: int(d * SR)]
    y *= 0.7 + 0.3 * np.sin(2 * np.pi * 18 * t(d))
    ring = tone(d, 1240, 0.12, start=0.03) + 0.5 * tone(d, 1910, 0.08, start=0.03)
    return y + 0.08 * ring


def lob():
    return whoosh(0.3, 600, 2000, 0.12) * 0.8


def slam():
    d = 0.75
    thud = glide(d, 120, 38, 0.6) * env(d, 0.003, 0.18)
    burst = lp(noise(d), 900) * env(d, 0.002, 0.05)
    rubble = crackle(d, 120, 300, 2500, 0.006, fade=0.18)
    return room(1.0 * thud + 0.8 * burst + 0.35 * rubble, 0.25)


def cast_poison():
    d = 0.55
    slosh = lp(noise(d), 700) * bell(d, 0.15, 0.08)
    return room(0.6 * bubbles(d, 14) + 0.6 * slosh, 0.15)


def cast_holy():
    d = 0.8
    y = sum(tone(d, f, 0.35, attack=0.03, detune=dt) for f in (880, 1108.7, 1318.5, 1760) for dt in (0, 0.003))
    shimmer = hp(noise(d), 6000) * bell(d, 0.1, 0.06)
    return room(0.35 * y + 0.3 * shimmer, 0.3)


def aura():
    d = 0.85
    pad = sum(glide(d, f, f * 1.5, 0.7) for f in (220, 277, 330)) * bell(d, 0.4, 0.2)
    air = swept(noise(d), 1500, 7000, "highpass") * bell(d, 0.45, 0.18)
    return room(0.3 * pad + 0.25 * air, 0.3)


def heal():
    d = 0.9
    notes = [523.3, 659.3, 784.0, 1046.5]
    y = sum(tone(d, f, 0.3, attack=0.01, start=0.07 * i) + 0.3 * tone(d, f * 2, 0.15, start=0.07 * i) for i, f in enumerate(notes))
    return room(0.4 * y, 0.3)


def hit_fire():
    d = 0.32
    puff = swept(noise(d), 2500, 300) * env(d, 0.002, 0.07)
    return puff + 0.5 * crackle(d, 80, 1500, 6000, 0.002, fade=0.1)


def hit_lightning():
    d = 0.25
    crack = hp(noise(d), 1500) * env(d, 0.0005, 0.025)
    buzz = signal.square(2 * np.pi * 110 * t(d)) * env(d, 0.002, 0.05)
    return crack + 0.25 * lp(buzz, 2000)


def hit_poison():
    d = 0.3
    splat = bp(noise(d), 200, 1400) * env(d, 0.002, 0.05)
    return splat + 0.5 * bubbles(d, 5, 300, 800, 0.12)


def boom():
    d = 1.1
    blast = swept(noise(d), 4000, 150) * env(d, 0.003, 0.22)
    sub = glide(d, 70, 28) * env(d, 0.005, 0.3)
    tail = crackle(d, 60, 400, 3000, 0.005, fade=0.35)
    return room(blast + 0.9 * sub + 0.35 * tail, 0.3)


def level_up():
    d = 1.5
    notes = [392.0, 523.3, 659.3, 784.0, 1046.5]
    y = sum(tone(d, f, 0.55, attack=0.008, start=0.085 * i) + 0.35 * tone(d, f * 2, 0.3, start=0.085 * i)
            + 0.2 * tone(d, f * 3, 0.15, start=0.085 * i) for i, f in enumerate(notes))
    swell = swept(noise(d), 1200, 8000, "highpass") * bell(d, 0.42, 0.16)
    return room(0.35 * y + 0.18 * swell, 0.35)


# ---- made here ----------------------------------------------------------------------------------

def blink():
    """A blink or a portal: a quick rising shimmer sucked in, then a soft pop."""
    d = 0.5
    x = t(d)
    rise = (glide(d, 400, 1800, 1.4) * 0.5 + glide(d, 603, 2700, 1.4) * 0.3) * bell(d, 0.16, 0.07)
    air = swept(noise(d), 800, 7000, "highpass") * bell(d, 0.17, 0.06) * 0.5
    pop = (tone(d, 1320, 0.08, start=0.22) + 0.5 * tone(d, 1980, 0.05, start=0.22)) * 0.5
    thump = glide(d, 180, 60) * np.where(x >= 0.22, env(d, 0.002, 0.06), 0) * 0.4
    return room(rise + air + pop + thump, 0.25)


def beam():
    """A column of light landing: a short rush, a bright blast and a humming ring dying away."""
    lead, d = 0.25, 1.1
    x = t(lead)
    rush = swept(noise(lead), 600, 7000) * (x / lead) ** 2 * 0.45
    x = t(d)
    blast = swept(noise(d), 7000, 250) * env(d, 0.003, 0.18)
    sub = glide(d, 90, 35) * env(d, 0.005, 0.35) * 0.8
    hum = (np.sin(2 * np.pi * 110 * x) + 0.6 * np.sin(2 * np.pi * 165 * x) + 0.4 * np.sin(2 * np.pi * 220 * x)) * env(d, 0.02, 0.4) * 0.3
    ring = sum(tone(d, f, 0.5, attack=0.01) for f in (880, 1318.5, 1760)) * 0.12
    return room(np.concatenate([rush, blast + sub + hum + ring]), 0.35)


def cast_shadow():
    """Darkness gathering: a low breath swelling and drawn back in, under a dull, wavering drone."""
    d = 0.7
    x = t(d)
    breath = swept(noise(d), 300, 1400) * bell(d, 0.3, 0.13)
    drone = sum(np.sin(2 * np.pi * f * x * (1 + 0.02 * np.sin(2 * np.pi * 5 * x))) for f in (73.4, 77.8, 110)) * bell(d, 0.32, 0.16)
    hiss = hp(noise(d), 4000) * bell(d, 0.35, 0.05) * 0.25
    return room(0.7 * breath + 0.4 * lp(drone, 600) + hiss, 0.35)


def hit_shadow():
    """A shadow landing: a muffled thump and a dark, sucking tail."""
    d = 0.45
    thump = glide(d, 140, 45) * env(d, 0.002, 0.08)
    body = lp(noise(d), 600) * env(d, 0.002, 0.05)
    tail = swept(noise(d), 1500, 200) * env(d, 0.03, 0.12) * 0.5
    return room(thump + 0.6 * body + tail, 0.3)


def cast_arcane():
    """Arcane power gathering: bright detuned partials twinkling up over a soft rising air."""
    d = 0.7
    x = t(d)
    tw = sum(np.sin(2 * np.pi * f * x * (1 + 0.25 * (x / d))) * (0.5 + 0.5 * np.sin(2 * np.pi * (9 + 3 * i) * x))
             for i, f in enumerate((987.8, 1318.5, 1661.2, 2217.5))) * bell(d, 0.3, 0.14)
    air = swept(noise(d), 1200, 6000, "highpass") * bell(d, 0.3, 0.12)
    return room(0.25 * tw + 0.3 * air, 0.3)


def hit_arcane():
    """An arcane blast: a glassy chime over a soft burst."""
    d = 0.6
    burst = swept(noise(d), 6000, 500) * env(d, 0.002, 0.07)
    chime = sum(tone(d, f, 0.22, detune=dt) for f in (1046.5, 1568.0, 2093.0) for dt in (0, 0.006)) * 0.25
    sub = glide(d, 160, 60) * env(d, 0.003, 0.09) * 0.5
    return room(burst + chime + sub, 0.3)


def cast_water():
    """Water rising and pouring: a rushing swell with bubbles through it."""
    d = 0.7
    rush = bp(noise(d), 300, 2500) * bell(d, 0.3, 0.15) * (0.75 + 0.25 * np.sin(2 * np.pi * 7 * t(d)))
    return room(0.7 * rush + 0.45 * bubbles(d, 18, 300, 900), 0.2)


def hit_water():
    """A splash: a sharp wet burst, droplets falling after it."""
    d = 0.55
    splash = swept(noise(d), 5000, 600) * env(d, 0.002, 0.08)
    body = bp(noise(d), 200, 900) * env(d, 0.003, 0.06)
    drops = crackle(d, 90, 1500, 6000, 0.003, fade=0.2) + 0.4 * bubbles(d, 8, 500, 1200, 0.35)
    return room(splash + 0.6 * body + 0.5 * drops, 0.2)


def cast_nature():
    """Leaves and branches: a rustling gust with twigs creaking through it."""
    d = 0.7
    rustle = bp(noise(d), 1500, 7000) * bell(d, 0.32, 0.15) * (0.6 + 0.4 * np.abs(np.sin(2 * np.pi * 11 * t(d))))
    twigs = crackle(d, 50, 600, 3000, 0.004) * bell(d, 0.35, 0.18)
    wood = glide(d, 240, 190) * bell(d, 0.4, 0.08) * 0.15
    return room(0.6 * rustle + 0.6 * twigs + wood, 0.2)


def rumble():
    """The earth stirring: a low grinding roll with grit tumbling through it."""
    d = 0.8
    roll = lp(noise(d), 220) * bell(d, 0.3, 0.16) * 2.0
    grind = bp(noise(d), 300, 1200) * bell(d, 0.3, 0.14) * (0.6 + 0.4 * np.sin(2 * np.pi * 13 * t(d)))
    grit = crackle(d, 140, 500, 3500, 0.004) * bell(d, 0.35, 0.18)
    return room(roll + 0.4 * grind + 0.5 * grit, 0.3)


# name: (sound, peak)
SOUNDS = {
    "whirl": (whirl, 0.6),
    "throw": (throw, 0.65),
    "lob": (lob, 0.55),
    "slam": (slam, 0.9),
    "cast_poison": (cast_poison, 0.6),
    "cast_holy": (cast_holy, 0.55),
    "aura": (aura, 0.55),
    "heal": (heal, 0.55),
    "hit_fire": (hit_fire, 0.55),
    "hit_lightning": (hit_lightning, 0.55),
    "hit_poison": (hit_poison, 0.5),
    "boom": (boom, 0.9),
    "level_up": (level_up, 0.7),
    "blink": (blink, 0.6),
    "beam": (beam, 0.85),
    "cast_shadow": (cast_shadow, 0.6),
    "hit_shadow": (hit_shadow, 0.75),
    "cast_arcane": (cast_arcane, 0.55),
    "hit_arcane": (hit_arcane, 0.7),
    "cast_water": (cast_water, 0.6),
    "hit_water": (hit_water, 0.7),
    "cast_nature": (cast_nature, 0.55),
    "rumble": (rumble, 0.8),
}


def write(path, x):
    pcm = (np.clip(x, -1, 1) * 32767).astype("<i2")
    with wave.open(str(path), "wb") as f:
        f.setnchannels(1)
        f.setsampwidth(2)
        f.setframerate(SR)
        f.writeframes(pcm.tobytes())


def main():
    names = sys.argv[1:] or list(SOUNDS)
    OUT.mkdir(parents=True, exist_ok=True)
    for name in names:
        make, peak = SOUNDS[name]
        rng.bit_generator.state = np.random.default_rng(sum(map(ord, name))).bit_generator.state  # each its own seed
        x = finish(np.asarray(make(), dtype=float), peak)
        write(OUT / f"{name}.wav", x)
        print(f"{name}: {len(x) / SR:.2f} s")


if __name__ == "__main__":
    main()
