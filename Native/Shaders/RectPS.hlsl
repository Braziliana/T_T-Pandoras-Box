struct PixelShaderInput
{
    float4 position : SV_POSITION;
    float2 uv : TEXCOORD;
    float4 color : COLOR;
};

float4 main(PixelShaderInput input) : SV_TARGET
{
    return input.color;
}