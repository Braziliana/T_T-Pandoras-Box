#pragma pack_matrix(row_major)
cbuffer MatrixBuffer
{
    float4x4 viewProjectionMatrix;
};

struct Vertex
{
    float3 position : POSITION;
    float2 uv : TEXCOORD;
    float4 color : COLOR;
};

struct Instance
{
    float3 center : INSTANCEPOSITION;
    float3 scale : INSTANCESCALE;
    float4 color : INSTANCECOLOR;
};

struct VertexShaderOutput
{
    float4 position : SV_POSITION;
    float2 uv : TEXCOORD;
    float4 color : COLOR;
};

float4x4 ComputeWorldMatrix(float3 center, float3 scale)
{
    float4x4 translation = float4x4(1, 0, 0, center.x,
                                    0, 1, 0, center.y,
                                    0, 0, 1, center.z,
                                    0, 0, 0, 1);

    float4x4 scaling = float4x4(scale.x, 0, 0, 0,
                                0, scale.y, 0, 0,
                                0, 0, scale.z, 0,
                                0, 0, 0, 1);

    return mul(scaling, translation);
}

VertexShaderOutput main(Vertex vertex, Instance instance)
{
    VertexShaderOutput output;
    float4x4 world = ComputeWorldMatrix(instance.center, instance.scale);
    float4x4 worldViewProj = mul(world, viewProjectionMatrix);
    float4 clipPosition = mul(float4(vertex.position, 1.0f), worldViewProj);
  
    output.position = clipPosition;//mul(float4(worldPosition, 1.0f), viewProjectionMatrix);
    
    output.uv = vertex.uv;

    output.color = vertex.color * instance.color;

    return output;
}