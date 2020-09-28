import sys
import os.path
import subprocess

# The script we want to run is in the same directory as this Python script.
SCRIPTS_DIR_PATH = os.path.dirname(os.path.realpath(__file__))

# Within that scripts directory, the script has the following name.
NODE_SCRIPT_NAME = "node-script.js"

# Location of the Node.js executable.
NODE_PATH = "C:\\Program Files\\nodejs\\node.exe"

# This is how we invoke Node.js on a particular script, forwarding the current
# process's commandline arguments. We also make sure Node.js won't flash a
# console window.
def run_node_script(script_path):
    # We want to invoke a script with Node.js.
    args = [ NODE_PATH, script_path ]

    # We'll forward all our command line arguments to the script. Except, the
    # first element of sys.argv isn't really a command line argument; it's the
    # path to the Python interpretter that's executing THIS script.
    argv_iter = iter(sys.argv)
    next(argv_iter)
    for arg in argv_iter:
        args.append(arg)

    # We don't want to flash a console window. Using the .pyw extension for this
    # script helps, but there's the potential for Node.js to spawn its own
    # window. The following process creation flag prevents a window from being
    # created.
    CREATE_NO_WINDOW = 0x08000000

    # Invoke Node.js on our script.
    subprocess.call(args, creationflags=CREATE_NO_WINDOW)


if __name__ == "__main__":
    run_node_script(os.path.join(SCRIPTS_DIR_PATH, NODE_SCRIPT_NAME))
