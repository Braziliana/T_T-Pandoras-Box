using System.Numerics;

namespace Api
{
    public enum TextHorizontalOffset
    {
        None,
        Left,
        Center,
        Right
    }

    public enum TextVerticalOffset
    {
        None,
        Top,
        Center,
        Bottom
    }
    
    public interface IRenderer
    {
        public void Init();
        public delegate void OnRenderDelegate(float deltaTime);
        public event OnRenderDelegate? OnRender;
        public void RectFilled2D(Vector2 position, Vector2 size, Color color);
        public void RectFilled3D(Vector3 position, Vector2 size, Color color);
        public void RectFilledBordered2D(Vector2 position, Vector2 size, Color color, Color borderColor,
            float borderSize);
        public void RectFilledBordered3D(Vector3 position, Vector2 size, Color color, Color borderColor,
            float borderSize);
        public void RectBorder2D(Vector2 position, Vector2 size, Color color, float borderSize);
        public void RectBorder3D(Vector3 position, Vector2 size, Color color, float borderSize);
        
        public void CircleFilled2D(Vector2 position, float size, Color color);
        public void CircleFilled3D(Vector3 position, float size, Color color);
        public void CircleFilledBordered2D(Vector2 position, float size, Color color, Color borderColor,
            float borderSize);
        public void CircleFilledBordered3D(Vector3 position, float size, Color color, Color borderColor,
            float borderSize);
        public void CircleBorder2D(Vector2 position, float size, Color color, float borderSize);
        public void CircleBorder3D(Vector3 position, float size, Color color, float borderSize);
        
        public void Text(string text, Vector2 position, float size, Color color);
        public void Text(string text, Vector2 start, Vector2 end, float size, Color color);
        
        public void Text(string text, Vector2 position, float size, Color color,
            TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset);
        public void Text(string text, Vector2 start, Vector2 end, float size, Color color,
            TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset);
        bool IsOnScreen(Vector2 position);
        void SetProjectionViewMatrix(Matrix4x4 matrix4X4);
    }
}