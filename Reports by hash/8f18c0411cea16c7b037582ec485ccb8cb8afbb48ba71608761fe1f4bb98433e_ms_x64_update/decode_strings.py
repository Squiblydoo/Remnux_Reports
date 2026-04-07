import urllib.parse

# Custom base64 alphabet from _0x23df20 in film.js
ALPHA = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789+/='

def custom_b64decode(s):
    """Replicates the _0x23df20 function from film.js"""
    out = ''
    buf = 0
    nbits = 0

    for char in s:
        idx = ALPHA.find(char)
        if idx == -1 or char == '=':
            continue
        # Accumulate 6 bits
        buf = buf * 64 + idx
        nbits += 6
        if nbits >= 8:
            nbits -= 8
            byte_val = (buf >> nbits) & 0xFF
            out += '%' + format(byte_val, '02X')
            buf &= (1 << nbits) - 1

    try:
        return urllib.parse.unquote(out)
    except:
        return out

# The 10 strings from _0x17e6() before shuffling
strings = [
    'mta5nJe0wefHuuj0',
    'nJe3nZqWCKnovKnY',
    'ntC2otK2m0TWAMv4uG',
    'mJa2nZaZzu9SvMry',
    'nZyXmZC5wMjttfzX',
    'ntvlt2HruM8',
    'ndq4nZK2svPNwMvl',
    'mZuWndnZwe1MENi',
    'ogjhBg5KzG',
    'W5RnMog3Oq',
]

print("Decoded strings from _0x17e6():")
for i, s in enumerate(strings):
    decoded = custom_b64decode(s)
    print(f"  [{i}] '{s}' -> '{decoded}'")
