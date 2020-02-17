#version 450

struct MapUV
{
    vec3 X;
    int Set;
    vec3 Y;
};

layout (set=0, binding=0) uniform UnlitMaterialArguments
{
    vec4 BaseColorFactor;
    float AlphaCutoff;
    MapUV BaseColorMapUV; 
};

layout (set=0, binding=1) uniform NodeProperties
{
    mat4 WorldTransform;
};

layout (set=0, binding=2) uniform ViewProjection
{
    mat4 View;
    mat4 Projection;
};

void main()
{
}