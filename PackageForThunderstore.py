import argparse
import sys
import os
import shutil
import json

parser = argparse.ArgumentParser()
parser.add_argument("-n", "--name", type=str)
parser.add_argument("-v", "--version", type=str)
parser.add_argument("-o", "--outputpath", type=str)
# parser.add_argument("")

args: argparse.Namespace = parser.parse_args()
name: str = args.name
version: str = args.version
outputPath: str = args.outputpath

currentPath: str = sys.path[0]
packagePath: str = os.path.join(currentPath, "ThunderstorePackages")

if not os.path.isdir(packagePath):
    os.mkdir(packagePath)

modNamePath: str = os.path.join(packagePath, name)

if not os.path.isdir(modNamePath):
    os.mkdir(modNamePath)

if not (name == None or version == None or outputPath == None):

    plugins: str = os.path.join(modNamePath, "plugins")

    if os.path.isdir(plugins):
        shutil.rmtree(plugins)

    os.mkdir(plugins)

    # copy the built files
    shutil.copytree(outputPath, plugins, dirs_exist_ok=True)

    # create manifest if it doesnt exist
    
    with open(os.path.join(modNamePath, "manifest.json"), "a+") as f:
        # check if file is empty, if so write something
        print(f)
        try:
            manifest = json.load(f)
        except json.JSONDecodeError:
            manifest = {}

        # update version
        manifest["version_number"] = f"{version}"

        # null check everything else
        if manifest.get("name") == None or manifest.get("name") == "":
            manifest["name"] = f"{name}"

        if manifest.get("website_url") == None or manifest.get("website_url") == "":
            manifest["website_url"] = f"https://github.com/Mangochicken13/BoplMods"

        if manifest.get("description") == None:
            manifest["description"] = ""

        if manifest.get("dependencies") == None:
            manifest["dependencies"] = []
        
        #print(manifest)

    # write updated manifest, with really big files this is a bad idea, but the files here are pretty small
    with open(os.path.join(modNamePath, "manifest.json"), "w") as f:
        json.dump(manifest, f, indent=4)

    # copy default icon if none exists
    if not os.path.exists(os.path.join(modNamePath, "icon.png")):
        #print(sys.path)
        shutil.copyfile(os.path.join(sys.path[0], "icon.png"), os.path.join(modNamePath, "icon.png"))
    
    if not os.path.exists(os.path.join(modNamePath, "README.md")):
        f = open(os.path.join(modNamePath, "README.MD"), "x")
        f.close()