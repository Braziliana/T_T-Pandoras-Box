cbuffer MatrixBuffer : register(b0)
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
    float4x4 WorldMat : INSTANCE_TRANSFORM;
    float4 color : INSTANCECOLOR;
};

struct VertexShaderOutput
{
    float4 position : SV_POSITION;
    float2 uv : TEXCOORD;
    float4 color : COLOR;
};



VertexShaderOutput main(Vertex vertex, Instance instance)
{
    VertexShaderOutput output;
    output.position = float4(vertex.position, 1.0f);

    // output.position = mul( vertex.position, instance.WorldMat );
    // output.position = mul( output.position, viewProjectionMatrix );
    
    
    output.uv = vertex.uv;

    output.color = float4(1, 0, 0, 0);

    return output;
}