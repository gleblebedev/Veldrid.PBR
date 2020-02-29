#version 450

struct MapUV
{
    vec3 X;
    int Set;
    vec3 Y;
};

struct UnlitMaterialArguments
{
    vec4 BaseColorFactor;
    MapUV BaseColorMapUV; 
    float AlphaCutoff;
};

struct MetallicRoughnessMaterialArguments
{
    vec4 BaseColorFactor;
    MapUV BaseColorMapUV; 
    MapUV MetallicRoughnessUV; 
    MapUV NormalUV; 
    MapUV EmissiveUV; 
    MapUV OcclusionUV; 
    float MetallicFactor;
    float RoughnessFactor;
    float AlphaCutoff;
};

struct SpecularGlossinessMaterialArguments
{
    vec4 DiffuseFactor;
    MapUV DiffuseMapUV; 
    MapUV SpecularGlossinessUV; 
    MapUV NormalUV; 
    MapUV EmissiveUV; 
    MapUV OcclusionUV; 
    vec3 SpecularFactor;
    float GlossinessFactor;
    float AlphaCutoff;
};

layout (set=0, binding=0) uniform UnlitMaterialArgumentsUniform
{
    UnlitMaterialArguments unlitArgs;
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

layout (set=0, binding=3) uniform MetallicRoughnessMaterialArgumentsUniform
{
    MetallicRoughnessMaterialArguments metallicRoughnessArgs;
};
layout (set=0, binding=4) uniform SpecularGlossinessMaterialArgumentsUniform
{
    SpecularGlossinessMaterialArguments specularGlossinessArgs;
};


void main()
{
}