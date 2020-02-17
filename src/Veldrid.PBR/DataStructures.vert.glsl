#version 450

struct MapUV
{
    vec3 X;
    int Set;
    vec3 Y;
};

layout (set=0, binding=0) uniform UnlitMaterialArguments
{
    MapUV BaseColorMapUV; 
};

layout (set=0, binding=1) uniform NodeProperties
{
    mat4 WorldTransform;
};

void main()
{
}