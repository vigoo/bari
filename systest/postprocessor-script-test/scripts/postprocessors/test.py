import os

print 'Target dir:' + targetDir

genExe = get_tool("fsrepo://MessageGenerator/*.*", "MessageGenerator.exe")

cmd = genExe + ' "' + targetDir+'\\generated.txt" "Hello world\n"'
print cmd
os.system(cmd)

# with open(targetDir+'\\generated.txt', "w") as f:
# f.write('Hello world\n')

results = ['generated.txt']
