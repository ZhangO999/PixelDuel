#!/usr/bin/env python3
"""Headless port of the game's physics, driven straight off the constants in
the C# so the two cannot drift apart. Answers the questions you would normally
answer by playing: is the jump tall enough, is every ledge reachable, can a
fast fall punch through the floor."""
import re, io, os, math

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
def src(f): return io.open(os.path.join(ROOT, 'Assets/Scripts', f), encoding='utf-8').read()

# ---- pull the level and the tuning numbers out of the C# ----
LVL = src('Level.cs')
MAP = re.findall(r'"([#=.12]{5,})"', LVL[LVL.index('string[] Map'):LVL.index('public static int W')])
H, W = len(MAP), len(MAP[0])

P = src('Player.cs')
K = {n: float(v) for n, v in re.findall(r'const float (\w+)\s*=\s*(-?[\d.]+)f;', P)}
MAXSTEP = float(re.search(r'MaxStep = ([\d.]+)f', LVL).group(1))
HALFW   = float(re.search(r'HalfW = ([\d.]+)f', LVL).group(1))
HEIGHT  = float(re.search(r'Height = ([\d.]+)f', LVL).group(1))

def at(c, r):    return '#' if c < 0 or c >= W or r < 0 or r >= H else MAP[r][c]
def solid(c, r): return at(c, r) == '#'
def oneway(c, r):return at(c, r) == '='
def col_at(x):   return math.floor(x)
def row_at(y):   return H - 1 - math.floor(y)
def tile_top(r): return H - r
def tile_bot(r): return H - 1 - r

E = 1e-4

class Body:
    def __init__(self, x, y):
        self.x, self.y, self.vx, self.vy = x, y, 0.0, 0.0
        self.grounded, self.drop = False, False

    def rows_cols(self):
        return (row_at(self.y + HEIGHT - E), row_at(self.y + E),
                col_at(self.x - HALFW + E), col_at(self.x + HALFW - E))

    def move_x(self, dx):
        if dx == 0: return
        self.x += dx
        rt, rb, cl, cr = self.rows_cols()
        for r in range(rt, rb + 1):
            for c in range(cl, cr + 1):
                if solid(c, r):
                    self.x = (c - HALFW) if dx > 0 else (c + 1 + HALFW)
                    self.vx = 0.0
                    return

    def move_y(self, dy):
        prev = self.y
        self.y += dy
        self.grounded = False
        if dy == 0: return
        rt, rb, cl, cr = self.rows_cols()
        for r in range(rt, rb + 1):
            for c in range(cl, cr + 1):
                if solid(c, r):
                    if dy > 0: self.y = tile_bot(r) - HEIGHT
                    else:      self.y = tile_top(r); self.grounded = True
                    self.vy = 0.0
                    return
        if dy < 0 and not self.drop:
            for r in range(rt, rb + 1):
                for c in range(cl, cr + 1):
                    if oneway(c, r):
                        top = tile_top(r)
                        if prev >= top - 1e-3 and self.y < top:
                            self.y, self.vy, self.grounded = top, 0.0, True
                            return

    def move(self, dt):
        speed = math.hypot(self.vx, self.vy)
        steps = max(1, math.ceil(speed * dt / MAXSTEP))
        sub = dt / steps
        for _ in range(steps):
            self.move_x(self.vx * sub)
            self.move_y(self.vy * sub)

def move_towards(cur, tgt, delta):
    return tgt if abs(tgt - cur) <= delta else cur + math.copysign(delta, tgt - cur)

class Sim:
    def __init__(self, x, y):
        self.b = Body(x, y)
        self.coyote = self.buffer = self.drop_t = 0.0
        self.jump_held_last = False
        self.trace = []

    def step(self, dt, mx=0, jump=False, down=False):
        b = self.b
        jump_down = jump and not self.jump_held_last

        accel = K['GroundAccel'] if b.grounded else K['AirAccel']
        drag  = K['GroundFriction'] if b.grounded else K['AirFriction']
        if mx: b.vx = move_towards(b.vx, mx * K['RunSpeed'], accel * dt)
        else:  b.vx = move_towards(b.vx, 0.0, drag * dt)

        self.coyote = K['CoyoteTime'] if b.grounded else self.coyote - dt
        self.buffer = K['JumpBuffer'] if jump_down else self.buffer - dt

        dropped = False
        if jump_down and down and b.grounded:
            if oneway(col_at(b.x), row_at(b.y - 0.1)):
                self.drop_t = K['DropTime']; b.y -= 0.06; self.buffer = 0.0
                self.jump_held_last = True; dropped = True

        if not dropped:
            if self.buffer > 0 and self.coyote > 0:
                b.vy = K['JumpVel']; b.grounded = False
                self.buffer = self.coyote = 0.0
            if self.jump_held_last and not jump and b.vy > 0:
                b.vy *= K['JumpCut']
            self.jump_held_last = jump

        b.vy = max(b.vy - K['Gravity'] * dt, -K['MaxFall'])
        b.drop = self.drop_t > 0
        self.drop_t -= dt
        b.move(dt)
        self.trace.append((b.x, b.y))

DT = 1 / 60.0
ok = True
def check(name, passed, detail=''):
    global ok
    ok = ok and passed
    print(('  PASS  ' if passed else '  FAIL  ') + name + ('   ' + detail if detail else ''))

print('constants:', ', '.join(f'{k}={v:g}' for k, v in sorted(K.items())))
print(f'arena {W}x{H}, tiers at y =', sorted({tile_top(r) for r in range(H)
      for c in range(W) if oneway(c, r)}), 'floor y = 1\n')

# 1. jump heights
s = Sim(3.0, 1.0)
for _ in range(10): s.step(DT)
start = s.b.y
for _ in range(120): s.step(DT, jump=True)
apex = max(y for _, y in s.trace) - start
check('full jump clears a 3-unit tier', apex >= 3.15, f'apex {apex:.2f}')

s = Sim(3.0, 1.0)
for _ in range(10): s.step(DT)
s.step(DT, jump=True)
for _ in range(120): s.step(DT)
hop = max(y for _, y in s.trace) - 1.0
check('tapping jump gives a shorter hop', 0.4 < hop < apex * 0.75, f'hop {hop:.2f}')

# 2. every tier reachable from the one below
# derive the tier heights from the map rather than assuming them
tiers = [1.0] + sorted({float(tile_top(r)) for r in range(H)
                        for c in range(W) if oneway(c, r)})
def surfaces_at(y):
    r = H - int(y)
    return [c for c in range(W) if oneway(c, r) or solid(c, r)]

def can_reach(from_y, to_y):
    """Brute force: from any standing spot on the lower surface, is there a
    run-up direction and jump-hold length that lands on the upper surface?"""
    starts = surfaces_at(from_y)
    for x0 in starts:
        for direction in (-1, 0, 1):
            for hold in (6, 10, 14, 20, 30):
                s = Sim(x0 + 0.5, from_y)
                for _ in range(6): s.step(DT)
                if not s.b.grounded: break
                for i in range(150):
                    s.step(DT, mx=direction, jump=(i < hold))
                    if s.b.grounded and abs(s.b.y - to_y) < 0.01:
                        return True, x0 + 0.5, direction
    return False, None, None

for lo, hi in zip(tiers, tiers[1:]):
    got, x0, d = can_reach(lo, hi)
    check(f'y={lo:g} -> y={hi:g} reachable', got,
          f'from x={x0:.1f} heading {d:+d}' if got else '')

# 3. terminal-velocity fall must not punch through the floor.
# Drop down a column with no ledges in it, or the fall correctly stops early.
clear_cols = [c for c in range(1, W - 1)
              if not any(oneway(c, r) for r in range(H))]
DROP_X = clear_cols[0] + 0.5
worst = 1e9
for dt in (1/60.0, 1/45.0, 1/30.0):        # 1/30 is the clamp in Game.Update
    s = Sim(DROP_X, float(H) - 2.0)
    for _ in range(400): s.step(dt)
    worst = min(worst, min(y for _, y in s.trace))
    landed = s.b.grounded and abs(s.b.y - 1.0) < 1e-3
    check(f'full-speed fall lands on the floor at dt=1/{round(1/dt)}', landed,
          f'rest y={s.b.y:.3f}')
check('nothing ever ends up below the floor', worst >= 1.0 - 1e-3, f'lowest y {worst:.3f}')

# 4. one-way ledges: land on them from above, drop through with down+jump.
# Pick a column that actually has a ledge, rather than assuming one.
LEDGE_ROW = max(r for r in range(H) if any(oneway(c, r) for c in range(W)))
LEDGE_X = [c for c in range(W) if oneway(c, LEDGE_ROW)][1] + 0.5
LEDGE_Y = tile_top(LEDGE_ROW)
s = Sim(LEDGE_X, LEDGE_Y + 2.0)
for _ in range(200): s.step(DT)
check('falling onto a ledge lands on it', abs(s.b.y - LEDGE_Y) < 1e-3,
      f'y={s.b.y:.2f} (ledge at {LEDGE_Y:g})')
before = s.b.y
s.step(DT, down=True, jump=True)               # the drop-through input
for _ in range(200): s.step(DT, down=True)     # then just keep holding down
check('down + jump drops through the ledge to the floor',
      s.b.grounded and abs(s.b.y - 1.0) < 1e-3, f'from y={before:.0f} to y={s.b.y:.2f}')

# 5. jumping up through a ledge from underneath
s = Sim(LEDGE_X, 1.0)
for _ in range(6): s.step(DT)
for i in range(160): s.step(DT, jump=(i < 30))
check('you can jump up through a ledge and land on top',
      abs(s.b.y - LEDGE_Y) < 1e-3, f'y={s.b.y:.2f}')

# 6. horizontal reach of a running jump
s = Sim(2.0, 1.0)
for _ in range(30): s.step(DT, mx=1)           # get up to speed first
x0, air = s.b.x, 0.0
for i in range(80):
    s.step(DT, mx=1, jump=(i < 30))
    if not s.b.grounded: air = s.b.x - x0
    elif i > 2: break
print(f'\n  running jump covers {air:.2f} units horizontally before landing')
print(f'  run-up top speed {K["RunSpeed"]:g} u/s, airtime {2*K["JumpVel"]/K["Gravity"]:.2f}s')

print()
print('ALL PHYSICS CHECKS PASSED' if ok else 'PHYSICS CHECKS FAILED')
raise SystemExit(0 if ok else 1)
