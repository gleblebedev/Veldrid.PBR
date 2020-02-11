#version 450

struct UVTransform
{
    vec3 X;
    vec3 Y;
    int Set;
};

layout (set=0, binding=0) uniform AllTypes
{
    UVTransform S;
};

void main()
{
}