import os
import shutil

fsharp_filename = "FSharp.Core.dll"
fsharp_filename_lower = fsharp_filename.lower()

src = None
for dirpath, dirnames, filenames in os.walk(targetDir):
    if fsharp_filename_lower in [fn.lower() for fn in filenames]:
        src = os.path.join(dirpath, fn)
        break

if src is not None:
    dst = os.path.join(targetDir, fsharp_filename)
    if src != dst:
        shutil.move(src, dst)
        os.rmdir(os.path.dirname(src))

results = [fsharp_filename]
