#version 450

struct MapUV
{
    vec3 X;
    int Set;
    vec3 Y;
};

struct UnlitMaterialArguments
{
    MapUV BaseColorMapUV;
};

layout (set=0, binding=0) uniform AllTypes
{
    UnlitMaterialArguments S;
};

void main()
{
}