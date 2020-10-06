for i in *.vert *.frag
do
    glslc "$i" -o "$i.spv"
done