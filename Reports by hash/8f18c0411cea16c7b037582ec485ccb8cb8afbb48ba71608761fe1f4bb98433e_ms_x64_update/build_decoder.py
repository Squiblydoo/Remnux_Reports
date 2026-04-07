import re

with open('/home/remnux/mal/output/asar_extracted/film.js', 'rb') as f:
    content = f.read().decode('utf-8', errors='replace')

# Find end of the initial IIFE shuffler
iife_end = content.find(');') + 2

# Find _0x17e6 function bounds
h_match = re.search(r'function _0x17e6\(\)\{', content)
h_start = h_match.start()
h_brace_depth = 0
j = h_start
while j < len(content):
    if content[j] == '{':
        h_brace_depth += 1
    elif content[j] == '}':
        h_brace_depth -= 1
        if h_brace_depth == 0:
            h_end = j + 1
            break
    j += 1

# Find _0x1919 function bounds
fn_match = re.search(r'function _0x1919\(', content)
fn_start = fn_match.start()
brace_depth = 0
i = fn_start
while i < len(content):
    if content[i] == '{':
        brace_depth += 1
    elif content[i] == '}':
        brace_depth -= 1
        if brace_depth == 0:
            fn_end = i + 1
            break
    i += 1

# Find p0d1c1 array
p0d1c1_match = re.search(r'const p0d1c1=\[', content)
p0d_start = p0d1c1_match.start()
bracket_depth = 0
k = p0d_start + len('const p0d1c1=')
while k < len(content):
    if content[k] == '[':
        bracket_depth += 1
    elif content[k] == ']':
        bracket_depth -= 1
        if bracket_depth == 0:
            p0d_end = k + 2
            break
    k += 1

# Build safe decoder script
parts = [
    content[:iife_end],
    '\n',
    content[h_start:h_end],
    '\n',
    content[fn_start:fn_end],
    '\n',
    content[p0d_start:p0d_end],
    '\n',
    '// Dump all decoded strings\n',
    'const results = [];\n',
    'for(let idx = 0x100; idx < 0x800; idx++) {\n',
    '    try {\n',
    '        const s = _0x1919(idx, 0);\n',
    '        if(s && typeof s === "string" && s.length > 0) {\n',
    '            results.push("[" + idx + "] " + s);\n',
    '        }\n',
    '    } catch(e) {}\n',
    '}\n',
    'console.log(results.join("\\n"));\n',
]

node_script = ''.join(parts)

with open('/home/remnux/mal/output/decode_film.js', 'w') as out:
    out.write(node_script)
print(f"Written {len(node_script)} bytes to decode_film.js")
print(f"IIFE end: {iife_end}")
print(f"_0x17e6: {h_start} - {h_end}")
print(f"_0x1919: {fn_start} - {fn_end}")
print(f"p0d1c1: {p0d_start} - {p0d_end}")
