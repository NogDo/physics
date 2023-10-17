using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Personal_Project_Game.Physics;
using Personal_Project_Game.Object;

namespace Personal_Project_Game
{
    public enum Key
    {
        Left,
        Right,
        Up,
        Down,
        Tab
    };

    public partial class Form1 : Form
    {
        private Brush brush;
        private List<bool> list_isPress;

        private GameWorld gameWorld;

        private int padding;
        private List<Color> color;
        private List<Color> outlineColor;

        private Vector2[] vertexBuffer;

        private Vector2 startPoint;
        private Vector2 endPoint;
        private bool isDrag;

        public Form1()
        {
            InitializeComponent();
            this.Size = new Size(1280, 720);
            DoubleBuffered = true;

            padding = 100; // 패딩을 넣어서 생성된 도형이 폼 밖에서 그려지지 않게 한다.

            list_isPress = new List<bool>(); // 방향키가 눌렸는지 판단해줄 리스트
            for (int i = 0; i < 5; i++) // 초기값으로 모두 false를 넣는다.
            {
                list_isPress.Add(false);
            }

            color = new List<Color>(); // 도형 색
            outlineColor = new List<Color>(); // 도형 외곽선 색

            gameWorld = new GameWorld();
            if (!Rigidbody.CreateBoxBody(Width - padding, 100.0f, new Vector2(Width / 2 - padding, Height - padding), 1.0f, true, 0.5f, out Rigidbody ground, out string Error))
            {
                throw new Exception(Error); ;
            }

            // 사각형 강체 10개생성
            Random rand = new Random();
            for (int i = 0; i < 10; i++)
            {
                float width = (float)rand.NextDouble() * 50.0f + 20.0f;
                float height = (float)rand.NextDouble() * 50.0f + 20.0f;

                float positionX = (float)rand.NextDouble() * 120.0f + 1000.0f;
                float positionY = (float)rand.NextDouble() * 200.0f + 100.0f;

                if (!Rigidbody.CreateBoxBody(width, height, new Vector2(positionX, positionY), 2.0f, false, 0.6f, out Rigidbody box, out string error))
                {
                    throw new Exception(error);
                }

                this.gameWorld.AddRigidbody(box);
                this.color.Add(Color.FromArgb(200, 200, 200));
                this.outlineColor.Add(Color.FromArgb(50, 50, 50));
            }

            this.gameWorld.AddRigidbody(ground);
            this.color.Add(Color.FromArgb(60, 60, 60));
            this.outlineColor.Add(Color.FromArgb(255, 255, 255));

            startPoint = Vector2.zero;
            endPoint = Vector2.zero;
            isDrag = false;
        }

        /// <summary>
        /// Form이 로드 됐을때 실행되는 함수
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // 화면갱신 타이머와, 이동 및 물리계산 타이머를 시작한다.
            InvalidateTimer.Start();
        }

        /// <summary>
        /// 화면에 도형을 그리는 함수
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < this.gameWorld.BodyCount; i++)
            {
                if (!this.gameWorld.GetRigidbody(i, out Rigidbody rigidbody))
                {
                    throw new Exception("현재 입력하신 인덱스의 Rigidbody를 찾을 수 없습니다.");
                }

                float positionX = rigidbody.Position.x;
                float positionY = rigidbody.Position.y;

                brush = new SolidBrush(color[i]);

                if (rigidbody.shape == Shape.Circle)
                {
                    e.Graphics.FillEllipse(brush, positionX - rigidbody.radius, positionY - rigidbody.radius, rigidbody.radius * 2.0f, rigidbody.radius * 2.0f);
                    e.Graphics.DrawEllipse(new Pen(outlineColor[i], 1.5f), positionX - rigidbody.radius, positionY - rigidbody.radius, rigidbody.radius * 2.0f, rigidbody.radius * 2.0f);
                }
                else if (rigidbody.shape == Shape.Box)
                {
                    e.Graphics.FillPolygon(brush, Converter.toPointFArray(rigidbody.GetChangedVertex(), ref this.vertexBuffer));
                    e.Graphics.DrawPolygon(new Pen(outlineColor[i], 1.5f), Converter.toPointFArray(rigidbody.GetChangedVertex(), ref this.vertexBuffer));
                }
            }

            e.Graphics.DrawLine(new Pen(Color.Black), Converter.toPointF(startPoint), Converter.toPointF(endPoint));
            e.Graphics.DrawString("상자생성 >> R", new Font("Arial", 16), new SolidBrush(Color.Black), 500, 100);
        }

        /// <summary>
        /// 화면갱신 타이머
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InvalidateTick(object sender, EventArgs e)
        {
            PressKey();
            //WrapScreen();
            Invalidate();
        }

        /// <summary>
        /// 마우스 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            startPoint = new Vector2(e.X, e.Y);
            endPoint = startPoint;
            isDrag = true;
        }

        /// <summary>
        /// 마우스 드래그
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrag)
            {
                endPoint = new Vector2(e.X, e.Y);
            }
        }

        /// <summary>
        /// 마우스 떼기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            // 왼쪽 마우스를 뗐을 때 원을 생성하고, 마우스 처음 드래그 지점과 끝지점으로 방향과 힘을 계산해서 원을 날린다.
            if (e.Button.Equals(MouseButtons.Left))
            {
                Random rand = new Random();
                float radius = (float)rand.NextDouble() * 30.0f + 10.0f;

                if (!Rigidbody.CreateCircleBody(radius, new Vector2(startPoint.x, startPoint.y), 2.0f, false, 0.6f, out Rigidbody circle, out string error))
                {
                    throw new Exception(error);
                }

                this.gameWorld.AddRigidbody(circle);
                this.color.Add(Color.FromArgb(200, 200, 200));
                this.outlineColor.Add(Color.FromArgb(50, 50, 50));

                Vector2 amount = PhysicsMath.Normalize(startPoint - endPoint);
                float distance = PhysicsMath.Distance(startPoint, endPoint);
                circle.LinearVelocity = amount * distance / 10.0f;

                float j = -(1.0f + 0.6f) * PhysicsMath.Dot(circle.LinearVelocity, amount);
                j /= circle.inverseMass;

                Vector2 impulse = j * amount;

                circle.LinearVelocity -= impulse * circle.inverseMass;
            }

            isDrag = false;
            startPoint = Vector2.zero;
            endPoint = Vector2.zero;
        }

        /// <summary>
        /// 키보드의 특정키가 눌렸을 때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            KeyDecision(e, true);

            Random rand = new Random();
            if(e.KeyCode == Keys.R)
            {
                for (int i = 0; i < 10; i++)
                {
                    float width = (float)rand.NextDouble() * 50.0f + 20.0f;
                    float height = (float)rand.NextDouble() * 50.0f + 20.0f;

                    float positionX = (float)rand.NextDouble() * 120.0f + 1000.0f;
                    float positionY = (float)rand.NextDouble() * 200.0f + 100.0f;

                    if (!Rigidbody.CreateBoxBody(width, height, new Vector2(positionX, positionY), 2.0f, false, 0.6f, out Rigidbody box, out string error))
                    {
                        throw new Exception(error);
                    }

                    this.gameWorld.AddRigidbody(box);
                    this.color.Add(Color.FromArgb(200, 200, 200));
                    this.outlineColor.Add(Color.FromArgb(50, 50, 50));
                }
            }
        }

        /// <summary>
        /// 키보드의 특정키가 떼졌을 때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            KeyDecision(e, false);
        }

        /// <summary>
        /// 무슨 키가 눌려졌고, 떼졌는지 결정하는 함수
        /// </summary>
        /// <param name="e">키이벤트</param>
        /// <param name="isDown">눌렸는지 떼졌는지 판단</param>
        public void KeyDecision(KeyEventArgs e, bool isDown)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    list_isPress[(int)Key.Left] = isDown;
                    break;

                case Keys.Right:
                    list_isPress[(int)Key.Right] = isDown;
                    break;

                case Keys.Up:
                    list_isPress[(int)Key.Up] = isDown;
                    break;

                case Keys.Down:
                    list_isPress[(int)Key.Down] = isDown;
                    break;

                case Keys.Tab:
                    list_isPress[(int)Key.Tab] = isDown;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 키가 눌렸는지 판단하는 함수
        /// 현재는 키보드를 사용하지 않기 때문에 화면갱신시 계속 불러옴
        /// </summary>
        public void PressKey()
        {
#if false
            float dx = 0.0f;
            float dy = 0.0f;
            float forceMagnitude = 20000.0f;

            // 왼쪽 눌림
            if (list_isPress[(int)Key.Left])
            {
                dx--;
            }
            // 오른쪽 눌림
            if (list_isPress[(int)Key.Right])
            {
                dx++;
            }
            // 위 눌림
            if (list_isPress[(int)Key.Up])
            {
                dy--;
            }
            // 아래 눌림
            if (list_isPress[(int)Key.Down])
            {
                dy++;
            }

            if (!this.gameWorld.GetRigidbody(0, out Rigidbody rigidbody))
            {
                throw new Exception("현재 입력하신 인덱스의 Rigidbody를 찾을 수 없습니다.");
            }

            if (dx != 0 || dy != 0)
            {
                // 벡터상에서 대각선이동은 수직이동보다 더 많이 움직임. 고로 정규화를 해줘서 대각선에서도 똑같은 거리를 이동하게 해야함
                Vector2 forceDirection = PhysicsMath.Normalize(new Vector2(dx, dy));

                // 원래 여기에는 컴퓨터 성능에 맞게 velocity값이 일정해지는 time값을 곱해야하는데 구현하는 방법을 모르겠음.
                // 추가! 스탑워치 사용해서 이 코드를 실행하는데 까지 걸린 시간을 곱해줬는데 움직이는게 이상함.. 이방법은 아닌듯
                Vector2 force = forceDirection * forceMagnitude;

                rigidbody.AddForce(force);
            }
#endif
            if (list_isPress[(int)Key.Tab])
            {
                Console.WriteLine("현재 강체개수 : " + this.gameWorld.BodyCount);
            }

            // 원래는 이전 프레임과 지금 프레임의 경과시간의 차이를 넣는게 맞는데, 그걸 못구하겠어서 임의의 값을 넣어줌
            this.gameWorld.Step(0.2f, 1);

            for (int i = 0; i < this.gameWorld.BodyCount; i++)
            {
                if (!gameWorld.GetRigidbody(i, out Rigidbody rigidbody))
                {
                    throw new Exception("현재 입력하신 인덱스의 Rigidbody를 찾을 수 없습니다.");
                }

                AABB aabb = rigidbody.GetAABB();

                // 강체가 아래쪽을 벗어나면 강체를 삭제
                if (aabb.Max.y > Height)
                {
                    this.gameWorld.RemoveRigidbody(rigidbody);
                    this.color.RemoveAt(i);
                    this.outlineColor.RemoveAt(i);
                }
            }
        }

    }
}
