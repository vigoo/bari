# sourceSet contains the source set to be built

import os
from os.path import basename

name = ''
for file in sourceSet:
    print "Script is processing " + file
    name = basename(file)

    with open(file, "r") as f:
        msg="Hello_" + f.read()
        
        target = targetDir + os.path.sep + ".." + os.path.sep + ".." + os.path.sep + "src" + os.path.sep + "HelloWorld" + os.path.sep + "Dep" + os.path.sep + "cs" + os.path.sep + "dep.cs"
        with open(target, "w") as ft:
            ft.writelines([
                "namespace HelloWorld.Dep",
                "{",
                "  public class dep", 
                "  {",
                "    public static readonly string Prop = \"TEST\";",
                "  }"
                "}"
            ])

results = [target]
