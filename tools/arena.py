#!/usr/bin/env python3
"""Draw the title screen exactly as Unity will build it -- same map, same tile
art, same bitmap font, same HUD layout -- so the whole thing can be eyeballed
before the editor is ever opened."""
import re, io, os, math, sys
from PIL import Image
import preview as P

S = 3                                   # on-screen scale
T = 16                                  # texture pixels per tile
MAP, H, W = P.arrays(P.SRC), None, None
LVL = io.open(os.path.join(P.ROOT, 'Assets/Scripts/Level.cs'), encoding='utf-8').read()
MAP = re.findall(r'"([#=.12]{5,})"', LVL[LVL.index('string[] Map'):LVL.index('public static int W')])
H, W = len(MAP), len(MAP[0])
A = P.A

GAME = io.open(os.path.join(P.ROOT, 'Assets/Scripts/Game.cs'), encoding='utf-8').read()
FONT = io.open(os.path.join(P.ROOT, 'Assets/Scripts/PixelFont.cs'), encoding='utf-8').read()
GLYPH = dict(re.findall(r"\{ '(.)', \"([01/]+)\" \}", FONT))

def hexc(h): return tuple(int(h[i:i+2], 16) for i in (0, 2, 4))
INK   = hexc('f2f0e5'); DIM = hexc('9aa6c0')
P1C   = hexc('d4453f'); P2C = hexc('3f8fd4')
STRIP = hexc('110f1c'); BACK = hexc('2a2b40')

img = Image.new('RGB', (W * T * S, H * T * S))
px = img.load()

# backdrop gradient, same two colours as BuildBackdrop
top, bot = hexc('2a3a6b'), hexc('5e4a72')
for y in range(img.height):
    t = 1 - y / (img.height - 1)
    row = tuple(round(bot[i] + (top[i] - bot[i]) * t) for i in range(3))
    for x in range(img.width): px[x, y] = row

def blit(rows, pal, ox, oy, flip=False):
    """ox, oy are top-left in texture pixels."""
    for yy, r in enumerate(rows):
        r = r[::-1] if flip else r
        for xx, ch in enumerate(r):
            if ch == '.': continue
            c = pal.get(ch, (255, 0, 255))[:3]
            for sy in range(S):
                for sx in range(S):
                    X, Y = (ox + xx) * S + sx, (oy + yy) * S + sy
                    if 0 <= X < img.width and 0 <= Y < img.height: px[X, Y] = c

def rect(cx, cy, w, h, colour, alpha=1.0):
    """World-space centred rectangle, matching Game.Place()."""
    x0 = int((cx - w / 2) * T * S); x1 = int((cx + w / 2) * T * S)
    y0 = int((H - (cy + h / 2)) * T * S); y1 = int((H - (cy - h / 2)) * T * S)
    for Y in range(max(0, y0), min(img.height, y1)):
        for X in range(max(0, x0), min(img.width, x1)):
            if alpha >= 1: px[X, Y] = colour
            else:
                o = px[X, Y]
                px[X, Y] = tuple(round(o[i] + (colour[i] - o[i]) * alpha) for i in range(3))

def text(msg, colour, scale, cx, cy):
    msg = msg.upper()
    wpx = (len(msg) * 6 - 1) * scale
    x0 = (cx * T) - wpx / 2
    y0 = (H - cy) * T - (7 * scale) / 2
    for i, ch in enumerate(msg):
        g = GLYPH.get(ch, GLYPH[' ']).split('/')
        for gy in range(7):
            for gx in range(5):
                if g[gy][gx] != '1': continue
                for sy in range(scale):
                    for sx in range(scale):
                        bx = x0 + (i * 6 + gx) * scale + sx
                        by = y0 + gy * scale + sy
                        for ky in range(S):
                            for kx in range(S):
                                X, Y = int(bx * S) + kx, int(by * S) + ky
                                if 0 <= X < img.width and 0 <= Y < img.height:
                                    px[X, Y] = colour

# ---- tiles ----
base = P.palette(P.SRC, 0)
for r in range(H):
    for c in range(W):
        ch = MAP[r][c]
        if ch == '#':
            above_open = r > 0 and MAP[r - 1][c] != '#'
            blit(A['TileGrass'] if above_open else A['TileSolid'], base, c * T, r * T)
        elif ch == '=':
            blit(A['TileLedge'], base, c * T, r * T)

def blit_player(i, wx, wy, facing, pose='LegsIdle'):
    """Character plus gun, using the same offsets Player.Animate applies."""
    pal = P.palette(P.SRC, i)
    flip = facing < 0
    blit(A['Torso'] + A[pose], pal,
         int((wx - 0.5) * T), int((H - (wy + 1)) * T), flip=flip)
    g = A['GunArt']
    gw, gh = len(g[0]), len(g)
    pivx, pivy = 0.1 * gw, 0.4 * gh                 # Art.Gun() pivot, in texels
    hx = (wx + 0.30 * facing) * T                   # HandOffset from Player.cs
    hy = (wy + 0.41) * T
    left = hx - (gw - pivx if flip else pivx)
    blit(g, base, int(round(left)), int(round(H * T - (hy - pivy) - gh)), flip=flip)

PLAY = len(sys.argv) > 1 and sys.argv[1] == 'play'

if PLAY:
    blit_player(0, 3.4, 1.0, +1, 'LegsRun0')
    blit_player(1, 12.4, 4.0, -1, 'LegsIdle')
    blit_player(1, 0, 0, -1) if False else None
else:
    for i, want in enumerate('12'):
        for r in range(H):
            for c in range(W):
                if MAP[r][c] != want: continue
                blit_player(i, c + 0.5, H - 1 - r, 1 if i == 0 else -1)

# ---- HUD, mirroring BuildHud/UpdateHud ----
BarY, BarH, BarW = H - 0.5, 0.42, 6.0
B1L, B2R = 1.2, W - 1.2
rect(W / 2, BarY, W - 2, 0.78, STRIP, 0.9)
fracs = (0.66, 1.0) if PLAY else (1.0, 1.0)
for i, (anchor, col) in enumerate([(B1L, P1C), (B2R, P2C)]):
    rect(B1L + BarW / 2 if i == 0 else B2R - BarW / 2, BarY,
         BarW + 0.14, BarH + 0.14, BACK)
    w = BarW * fracs[i]
    rect(anchor + w / 2 if i == 0 else anchor - w / 2, BarY, w, BarH, col)
text('2 - 1' if PLAY else '0 - 0', INK, 1, W / 2, BarY)

# ---- title text, positions read out of Game.cs ----
for bx, by in ((5.6, 1.45), (7.0, 1.45), (11.2, 4.45)) if PLAY else ():
    blit(A['BulletArt'], base, int(bx * T), int(H * T - by * T), flip=False)

Mid = H * 0.5
if not PLAY:
  rect(W / 2, Mid + 1.1, W - 1.4, 5.9, hexc('0d0b16'), 210 / 255)
  text('PIXEL DUEL', INK, 3, W / 2, Mid + 3.0)
  text('P1  AD MOVE  WS AIM  LSHIFT JUMP  SPACE FIRE', P1C, 1, W / 2, Mid + 1.2)
  text('P2  MOVE AND AIM ARROWS   JUMP /   FIRE RSHIFT',   P2C, 1, W / 2, Mid + 0.4)
  text('FIRST TO 5   -   PRESS SPACE OR ENTER TO FIGHT',   DIM, 1, W / 2, Mid - 1.0)

out = os.path.join(P.ROOT, 'tools/arena_play.png' if PLAY else 'tools/arena.png')
img.save(out)
print('wrote', out, img.size)
