
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zappy.Properties;

namespace Zappy
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			this.InitializeComponent();

			this.Text = Dice(6);
			this.isTriggerbot = this.GetKey<bool>("isTriggerbot");
			this.isAimbot = this.GetKey<bool>("isAimbot");
			this.isEsp = this.GetKey<bool>("isEsp");
			this.fovX = this.GetKey<int>("fovX");
			this.fovY = this.GetKey<int>("fovY");
			this.TriggerRage = this.GetKey<bool>("TriggerRage");
			this.isCircle = this.GetKey<bool>("isCircle");
			this.speed = this.GetKey<decimal>("speed");
			this.speed3 = this.GetKey<decimal>("speed3");
			this.delayx = this.GetKey<decimal>("delayx");
			this.Bhop = this.GetKey<decimal>("Bhop");
			this.FovCircleRed = this.GetKey<int>("FovCircleRed");
			this.isRunning = this.GetKey<bool>("isRunning");
			this.FovCircleGreen = this.GetKey<int>("FovCircleGreen");
			this.FovCircleBlue = this.GetKey<int>("FovCircleBlue");
			this.FovCircleWidth = this.GetKey<int>("FovCircleWidth");
			this.color = (Form1.ColorType)this.GetKey<int>("color");
			this.mainAimKey = (Form1.AimKey)this.GetKey<int>("mainAimKey");
			this.Bhopxkey = (Form1.Bhopkey)this.GetKey<int>("Bhopxkey");
			this.isAimKey = this.GetKey<bool>("isAimKey");
			this.isHold = this.GetKey<bool>("isHold");
			this.monitor = this.GetKey<int>("monitor");
			this.offsetY = this.GetKey<int>("offsetY");
			this.msShootTime = this.GetKey<int>("msShootTime");
			this.isRecoil = this.GetKey<bool>("isRecoil");
			this.isBhop = this.GetKey<bool>("isBhop");
			this.PingX = this.GetKey<decimal>("PingX");
			Form1.ColorType colorType = this.color;

			if (colorType != Form1.ColorType.Red)
			{
				if (colorType == Form1.ColorType.Purple)
				{
					this.PurpleRadio.Checked = true;
				}
			}
			else
			{
				this.RedRadio.Checked = true;
			}
			this.UpdateUI();
			this.IsHoldToggle.Checked = this.isHold;
			this.AimbotBtt.Checked = this.isAimbot;
			this.EspBtt.Checked = this.isEsp;
			this.Ragebot.Checked = this.TriggerRage;
			this.CircleBtt.Checked = this.isCircle;
			this.Bhopbox.Checked = this.isBhop;
			this.AimKeyToggle.Checked = this.isAimKey;
			this.Speed.Value = this.speed;
			this.Speed3.Value = this.speed3;
			this.Delayx.Value = this.delayx;
			this.Bhopinput.Value = this.Bhop;
			this.FovXNum.Value = this.fovX;
			this.FovYNum.Value = this.fovY;
			this.CircleRed.Value = this.FovCircleRed;
			this.CircleGreen.Value = this.FovCircleGreen;
			this.CircleBlue.Value = this.FovCircleBlue;
			this.CircleWidth.Value = this.FovCircleWidth;
			this.TriggerbotBtt.Checked = this.isTriggerbot;
			this.offsetNum.Value = this.offsetY;
			this.FireRateNum.Value = this.msShootTime;
			foreach (string text in Enum.GetNames(typeof(Form1.AimKey)))
			{
				this.contextMenuStrip1.Items.Add(text);
			}
			this.contextMenuStrip1.ItemClicked += delegate (object o, ToolStripItemClickedEventArgs e) {
				this.mainAimKey = (Form1.AimKey)Enum.Parse(typeof(Form1.AimKey), e.ClickedItem.ToString());
				this.SetKey("mainAimKey", (int)this.mainAimKey);
				this.UpdateUI();
			};
			foreach (string text in Enum.GetNames(typeof(Form1.Bhopkey)))
			{
				this.contextMenuStrip2.Items.Add(text);
			}
			this.contextMenuStrip2.ItemClicked += delegate (object o, ToolStripItemClickedEventArgs e) {
				this.Bhopxkey = (Form1.Bhopkey)Enum.Parse(typeof(Form1.Bhopkey), e.ClickedItem.ToString());
				this.SetKey("Bhopxkey", (int)this.Bhopxkey);
				this.UpdateUI();
			};

			this.AutoSize = false;
			base.AutoScaleMode = AutoScaleMode.Font;
			this.Font = new Font("Trebuchet MS", 10f, FontStyle.Regular, GraphicsUnit.Point, 204);
		}

		[DllImport("gdi32.dll")]
		private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

		private static float GetScalingFactor()
		{
			IntPtr hdc = Graphics.FromHwnd(IntPtr.Zero).GetHdc();
			int deviceCaps = Form1.GetDeviceCaps(hdc, 10);
			return (float)Form1.GetDeviceCaps(hdc, 117) / (float)deviceCaps;
		}

		[DllImport("user32.dll")]
		private static extern short GetAsyncKeyState(int vKey);

		[DllImport("USER32.dll")]
		private static extern short GetKeyState(int nVirtKey);

		public int pubx;
		public int puby;

		private async void xAimbot()
		{

			for (; ; )
			{

				try
				{

					if (!this.isRunning || !this.isAimbot && !this.isEsp && !this.isTriggerbot && !this.TriggerRage)
					{
						await Task.Delay(1).ConfigureAwait(false);
					}
					else
					{
						bool pressDown = false;
						this.Move(0, 0, false);
						Color PixelColor;
						if (Customcolor.Checked == true)
						{
							int r = int.Parse(Redinput.Text);
							int g = int.Parse(Greeninput.Text);
							int b = int.Parse(Blueinput.Text);
							PixelColor = Color.FromArgb(r, g, b);
						}
						else
						{
							PixelColor = Color.FromArgb(GetColor(this.color));
						}

						if (this.isTriggerbot && !this.isAimbot && !this.TriggerRage && !this.isEsp)
						{
							if (this.isAimKey)
							{
								int keyState = (int)Form1.GetKeyState((int)this.Bhopxkey);
								if (this.isHold)
								{
									if (keyState >= 0)
									{
										await Task.Delay(1).ConfigureAwait(false);
										continue;
									}
								}
								else if (keyState != 0)
								{
									await Task.Delay(1).ConfigureAwait(false);
									continue;
								}
							}

							int toka = int.Parse(Pingx.Text);

							if (this.PixelSearch(new Rectangle((xSize - toka) / 2, (ySize - toka) / 2, toka, toka), PixelColor, this.colorVariation).Length != 0)
							{
								this.Move(0, 0, true);
								continue;
							}

						}

						Point[] array = this.PixelSearch(new Rectangle((this.xSize - this.fovX) / 2, (this.ySize - this.fovY) / 2, this.fovX, this.fovY), PixelColor, this.colorVariation);
						if (array.Length != 0)
						{
							try
							{
								Point[] array2 = (from t in array orderby t.Y select t).ToArray<Point>();
								List<Vector2> list = new List<Vector2>();

								for (int j = 0; j < array2.Length; j++)
								{

									if (this.isEsp)
									{
										int wi = int.Parse(ColWidth.Text);
										int u = int.Parse(ColR.Text);
										int l = int.Parse(ColG.Text);
										int m = int.Parse(ColB.Text);
										int sx = int.Parse(ColX.Text);
										int sy = int.Parse(ColY.Text);
										Color pol = Color.FromArgb(u, l, m);

										Pen Red = new Pen(pol)
										{
											Width = wi
										};

										using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
										{
											g.DrawRectangle(Red, array2[j].X - (sx / 1), array2[j].Y - 1, sx, sy);         //I know this is shitty drawing stop looking at it thanks xD
										}

									}

									Vector2 current = new Vector2((float)array2[j].X, (float)array2[j].Y);
									if (!(from t in list where (t - current).Length() < 20f || Math.Abs(t.X - current.X) < 20f select t).Any())
									{
										list.Add(current);
										if (list.Count > 0)
										{
											break;
										}
									}
								}

								int pixelx = Convert.ToInt32(SmoothX.Value);
								int ab = Convert.ToInt32(chanceval.Value);
								int val2 = (101 - ab) / 2;
								pressDown = false;
								int toka = int.Parse(Pingx.Text);
								antirecoilval = 0;

								Vector2 vector = (from t in list select t - new Vector2((float)(this.xSize / 2), (float)(this.ySize / 2)) into t orderby t.Length() select t).ElementAt(0) + new Vector2(1f, (float)this.offsetY);

								if (TargetCheck.Checked == false)
								{

									if (this.isTriggerbot)
									{
										if (this.isAimKey)
										{
											int keyState2 = (int)Form1.GetKeyState((int)this.Bhopxkey);
											if (this.isHold)
											{
												if (keyState2 >= 0)
												{
												}
												else
												{
													for (int i = 0; i < array.Length; i++)
													{
														
													}
												}
											}
											else if (keyState2 != 0)
											{
											}
											else
											{
												for (int i = 0; i < array.Length; i++)
												{

												}
											}
										}
									}
									if (this.TriggerRage)
									{
										for (int i = 0; i < array.Length; i++)
										{

										}
									}

									if (this.isAimbot || this.TriggerRage)
									{

										if (this.isAimKey)
										{
											int keyState = (int)Form1.GetKeyState((int)this.mainAimKey);
											if (this.isHold)
											{
												if (keyState >= 0)
												{
													continue;
												}
											}
											else if (keyState != 0)
											{
												continue;
											}
										}

										this.Move((int)(vector.X * (float)this.speed), (int)(vector.Y * (float)this.speed) + antirecoilval, pressDown);
										pressDown = false;
										continue;
									}
								}
								else
								{

									for (int i = 0; i < array.Length; i++)
									{

										if ((new Vector2((float)array[i].X, (float)array[i].Y) - new Vector2((float)(xSize / 2), (float)(ySize / 2))).Length() < pixelx)
										{
											slowmove = true;
										}
										else
										{
											slowmove = false;
										}
									}

									if (slowmove)
									{

										if (this.isTriggerbot)
										{
											if (this.isAimKey)
											{
												int keyState2 = (int)Form1.GetKeyState((int)this.Bhopxkey);
												if (this.isHold)
												{
													if (keyState2 >= 0)
													{
													}
													else
													{
														for (int i = 0; i < array.Length; i++)
														{

														}
													}
												}
												else if (keyState2 != 0)
												{
												}
												else
												{
													for (int i = 0; i < array.Length; i++)
													{

													}
												}
											}
										}
										if (this.TriggerRage)
										{
											for (int i = 0; i < array.Length; i++)
											{

											}
										}

										if (this.isAimbot || this.TriggerRage)
										{

											if (this.isAimKey)
											{
												int keyState = (int)Form1.GetKeyState((int)this.mainAimKey);
												if (this.isHold)
												{
													if (keyState >= 0)
													{
														continue;
													}
												}
												else if (keyState != 0)
												{
													continue;
												}
											}

											int x = Convert.ToInt32(this.delayx);
											await Task.Delay(x).ConfigureAwait(false);
											this.Move((int)(vector.X * (float)this.speed3), (int)(vector.Y * (float)this.speed3) + antirecoilval, pressDown);
											pressDown = false;
											continue;

										}
									}
									else
									{
										pressDown = false;
										this.Move((int)(vector.X * (float)this.speed), (int)(vector.Y * (float)this.speed), pressDown);
										continue;
									}

								}
								continue;

							}
							catch
							{
								continue;
							}
						}
					}
				}
				catch
				{

				}

			}
		}



		private async void XBhop() //Bhop spamming spacebar
		{
			for (; ; )
			{
				try
				{

				News:

					if (!this.isRunning || !this.isBhop)
					{
						await Task.Delay(1000).ConfigureAwait(false);
						goto News;
					}
					else
					{
						int xx = int.Parse(Bhopinput.Text);
						int keyState = (int)Form1.GetKeyState(xx);

						if (keyState >= 0)
						{
							await Task.Delay(10).ConfigureAwait(false);
							goto News;
						}
						else
						{
							int v = int.Parse(Bdelay.Text);
							SendKeys.SendWait(" ");
							Thread.Sleep(v);
						}

					}
				}
				catch
				{

				}
			}
		}

		private static Random random = new Random(); //Randomize Window Title
		public static string Dice(int length) { const string chars = "qwerasdfyxcv"; return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()); } //Randomize for further autostrafe testing

		private int dx;
		private int df;

		private async void XDice() //Change the Window Title name to something random
		{
			for (; ; )
			{
				if (!this.isRunning)
				{
					await Task.Delay(1000).ConfigureAwait(false);
				}
				else
				{
					Color PixelColor;
					if (Customcolor.Checked == true)
					{
						int r = int.Parse(Redinput.Text);
						int g = int.Parse(Greeninput.Text);
						int b = int.Parse(Blueinput.Text);
						PixelColor = Color.FromArgb(r, g, b);
					}
					else
					{
						PixelColor = Color.FromArgb(GetColor(this.color));
					}
					Random rnd = new Random();
					dx = rnd.Next(1500000, 2500000);
					df = rnd.Next(480000, 900000);

					await Task.Delay(df).ConfigureAwait(false);
					this.Text = Dice(6);
					Update();
					await Task.Delay(dx).ConfigureAwait(false);
				}
			}
		}



		public static int GetColor(Form1.ColorType color)
		{
			if (color == Form1.ColorType.Red)
			{
				return 0x8a4946;



			}
			if (color != Form1.ColorType.Purple)
			{
				return 0xe85dfe;
			}

			return 11480751;
		}

		private void UpdateDisplayInformation()
		{
			try
			{
				if (CustomScreen.Checked == true)
				{
					int x = int.Parse(ScreenX2.Text);
					int y = int.Parse(ScreenY2.Text);

					this.xSize = x;
					this.ySize = y;
				}
				else
				{
					this.zoom = GetScalingFactor();
					Screen screen = this.CurrentScreen();
					bool primary = screen.Primary;
					this.xSize = (int)((float)screen.Bounds.Width * (primary ? this.zoom : 1f));
					this.ySize = (int)((float)screen.Bounds.Height * (primary ? this.zoom : 1f));
				}
			}
			catch
			{
			

			}
		}

		[DllImport("user32.dll")]
		private static extern void mouse_event(int dwFlags, int dx, int dy, uint dwData, UIntPtr dwInformation);

		private MoveInfo _mevent;

		public new void Move(int x, int y, bool lm = false)
		{

			if (lm)
			{
				if (DateTime.Now.Subtract(this.lastShot).TotalMilliseconds < (double)this.msShootTime)
				{
					lm = false;
				}
				else
				{
					this.lastShot = DateTime.Now;
				}
			}
			_mevent = new MoveInfo
			{
				MovementSettings = (InternCaseMoveSettings)8196,
				xAmount = (int)x,
				yAmount = (int)y,
			};

			if (lm)
			{
				_mevent = new MoveInfo
				{
					MovementSettings = (InternCaseMoveSettings)8194,
				};
			}

			CaseExecute.ExecuteMovementCase(_mevent);
		}

		public Screen CurrentScreen()
		{
			return Screen.AllScreens[this.monitor];
		}

		public class DirectBitmap : IDisposable
		{
			public Bitmap Bitmap { get; private set; }

			private int[] bits;

			public int[] GetBits()
			{
				return bits;
			}

			private void SetBits(int[] value)
			{
				bits = value;
			}

			public bool Disposed { get; private set; }
			public int Height { get; private set; }
			public int Width { get; private set; }

			protected GCHandle BitsHandle { get; private set; }

			public DirectBitmap(int width, int height)
			{
				Width = width;
				Height = height;
				SetBits(new Int32[width * height]);
				BitsHandle = GCHandle.Alloc(GetBits(), GCHandleType.Pinned);
				Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
			}

			public void SetPixel(int x, int y, Color colour)
			{
				int index = x + (y * Width);
				int col = colour.ToArgb();

				GetBits()[index] = col;
			}

			public Color GetPixel(int x, int y)
			{
				int index = x + (y * Width);
				int col = GetBits()[index];
				Color result = Color.FromArgb(col);

				return result;
			}

			void IDisposable.Dispose()
			{
				if (Disposed) return;
				Disposed = true;
				Bitmap.Dispose();
				BitsHandle.Free();
			}
		}

		public unsafe Point[] PixelSearch(Rectangle rect, Color PixelColor, int ShadeVariation)
		{

			ArrayList arrayList = new ArrayList();
			using (var tile = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb))
			{
				if (this.monitor >= Screen.AllScreens.Length)
				{
					this.monitor = 0;
					this.UpdateUI();
				}
				int left = Screen.AllScreens[this.monitor].Bounds.Left;
				int top = Screen.AllScreens[this.monitor].Bounds.Top;
				using (var g = Graphics.FromImage(tile))
				{
					g.CopyFromScreen(rect.X + left, rect.Y + top, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);
				}
				BitmapData bitmapData = tile.LockBits(new Rectangle(0, 0, tile.Width, tile.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
				int[] array = new int[]
					{
					(int) PixelColor.B,
					(int) PixelColor.G,
					(int) PixelColor.R
				};

				for (int i = 0; i < bitmapData.Height; i++)
				{
					byte* ptr = (byte*)((void*)bitmapData.Scan0) + i * bitmapData.Stride;
					for (int j = 0; j < bitmapData.Width; j++)
					{
						if (((int)ptr[j * 3] >= array[0] - ShadeVariation & (int)ptr[j * 3] <= array[0] + ShadeVariation) && ((int)ptr[j * 3 + 1] >= array[1] - ShadeVariation & (int)ptr[j * 3 + 1] <= array[1] + ShadeVariation) && ((int)ptr[j * 3 + 2] >= array[2] - ShadeVariation & (int)ptr[j * 3 + 2] <= array[2] + ShadeVariation))
						{
							arrayList.Add(new Point(j + rect.X, i + rect.Y));
						}
					}
				}
				return (Point[])arrayList.ToArray(typeof(Point));

			}
		}

		private void Red_changed(object sender, EventArgs e)
		{
			this.color = Form1.ColorType.Red;
			this.SetKey("color", (int)this.color);
		}

		private void Purple_changed(object sender, EventArgs e)
		{
			this.color = Form1.ColorType.Purple;
			this.SetKey("color", (int)this.color);
		}

		private void Speed_changed(object sender, EventArgs e)
		{

			this.speed = this.Speed.Value;
			this.SetKey("speed", this.speed);

		}
		private void FovX_changed(object sender, EventArgs e)
		{


			this.fovX = (int)this.FovXNum.Value;
			this.SetKey("fovX", this.fovX);

		}


		private void FovYNum_ValueChanged(object sender, EventArgs e)
		{
			this.fovY = (int)this.FovYNum.Value;
			this.SetKey("fovY", this.fovY);

		}

		private void IsAimbot_changed(object sender, EventArgs e)
		{
			if (AimbotBtt.Checked)
			{
				Ragebot.Enabled = false;
				chanceval.Enabled = false;
				Firerage.Enabled = false;
				Ragebot.Checked = false;

			}
			else
			{
				Ragebot.Enabled = true;
				chanceval.Enabled = true;
				Firerage.Enabled = true;
			}

			this.isAimbot = this.AimbotBtt.Checked;
			this.SetKey("isAimbot", this.isAimbot);

		}

		private void CheckBox1_CheckedChanged(object sender, EventArgs e)
		{
			if (Ragebot.Checked)
			{
				AimbotBtt.Enabled = false;
				TriggerbotBtt.Enabled = false;
				FireRateNum.Enabled = false;
				AimbotBtt.Checked = false;
				TriggerbotBtt.Checked = false;

				this.msShootTime = (int)this.Firerage.Value;

			}
			else
			{
				AimbotBtt.Enabled = true;
				TriggerbotBtt.Enabled = true;
				FireRateNum.Enabled = true;
			}

			this.TriggerRage = Ragebot.Checked;
			this.SetKey("TriggerRage", this.TriggerRage);
		}

		private void IsTriggerbot_changed(object sender, EventArgs e)
		{
			if (TriggerbotBtt.Checked)
			{
				this.msShootTime = (int)this.FireRateNum.Value;
				this.SetKey("msShootTime", this.msShootTime);
			}

			this.isTriggerbot = this.TriggerbotBtt.Checked;
			this.SetKey("isTriggerbot", this.isTriggerbot);
		}

		private void Main_load(object sender, EventArgs e)
		{

			this.mainThread = new Thread(delegate () {
				this.XBhop();
				this.XDice();
				this.xAimbot();
			});
			this.mainThread.Start();
		}
		private void SetKey(string key, bool o)
		{
			Settings.Default[key] = o;
			Settings.Default.Save();
		}

		private void SetKey(string key, int o)
		{
			Settings.Default[key] = o;
			Settings.Default.Save();
		}

		private void SetKey(string key, decimal o)
		{
			Settings.Default[key] = o;
			Settings.Default.Save();
		}

		private T GetKey<T>(string key)
		{
			return (T)((object)Settings.Default[key]);
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			this.mainThread.Abort();
			Settings.Default.Save();
			base.OnHandleDestroyed(e);
		}



		private void Start_click(object sender, EventArgs e)
		{
			FormOverlay formoverlay = new FormOverlay();
			if (this.isRunning)
			{
				try
				{
					formoverlay.Close();
					FormOverlay obj = (FormOverlay)Application.OpenForms["FormOverlay"];
					obj.Close();
					CircleBtt.Checked = false;
				}
				catch { }
			}
			this.isRunning = !this.isRunning;
			this.UpdateUI();
			if (CircleBtt.Checked == true)
			{
				try { formoverlay.Show(); }
				catch { }
			}
		}

		private void UpdateUI()
		{
			this.StartBtt.Text = this.isRunning ? "Stop" : "Start";
			this.UpdateDisplayInformation();
			this.ChangeMonitorBtt.Text = string.Concat(new string[] {
				"Monitor [",
				this.monitor.ToString(),
				"] ",
				this.xSize.ToString(),
				"x",
				this.ySize.ToString()
			});
			this.AimkeyBtt.Text = Enum.GetName(typeof(Form1.AimKey), this.mainAimKey);
			this.TriggerKeyBtt.Text = Enum.GetName(typeof(Form1.Bhopkey), this.Bhopxkey);
		}

		private void MonitorChanged(object sender, EventArgs e)
		{
			this.monitor++;
			if (this.monitor >= Screen.AllScreens.Length)
			{
				this.monitor = 0;
			}
			this.SetKey("monitor", this.monitor);
			this.UpdateUI();
		}
		private void IsAimKeyChanged(object sender, EventArgs e)
		{
			this.isAimKey = this.AimKeyToggle.Checked;
			this.SetKey("isAimKey", this.isAimKey);
		}

		private void IsHold_changed(object sender, EventArgs e)
		{
			this.isHold = this.IsHoldToggle.Checked;
			this.SetKey("isHold", this.isHold);
		}

		private void AimKeyDrop(object sender, EventArgs e)
		{
			if (this.AimkeyBtt.PointToScreen(new Point(this.AimkeyBtt.Left, this.AimkeyBtt.Bottom)).Y + this.contextMenuStrip1.Size.Height > Screen.PrimaryScreen.WorkingArea.Height)
			{
				this.contextMenuStrip1.Show(this.AimkeyBtt, new Point(0, -this.contextMenuStrip1.Size.Height));
				return;
			}
			this.contextMenuStrip1.Show(this.AimkeyBtt, new Point(0, this.AimkeyBtt.Height));

		}

		private void TriggerKeyDrop(object sender, EventArgs e)
		{

			if (this.TriggerKeyBtt.PointToScreen(new Point(this.TriggerKeyBtt.Left, this.TriggerKeyBtt.Bottom)).Y + this.contextMenuStrip2.Size.Height > Screen.PrimaryScreen.WorkingArea.Height)
			{
				this.contextMenuStrip2.Show(this.TriggerKeyBtt, new Point(0, -this.contextMenuStrip2.Size.Height));
				return;
			}
			this.contextMenuStrip2.Show(this.TriggerKeyBtt, new Point(0, this.TriggerKeyBtt.Height));

		}

		private void OffsetY_changed(object sender, EventArgs e)
		{

			this.offsetY = (int)this.offsetNum.Value;
			this.SetKey("offsetY", this.offsetY);

		}


		private void FireRate_changed(object sender, EventArgs e)
		{
			if (isTriggerbot)
			{
				this.msShootTime = (int)this.FireRateNum.Value;
				this.SetKey("msShootTime", this.msShootTime);
			}
		}


		private enum AimKey
		{
			LeftMouse = 1,
			RightMouse,
			X1Mouse = 5,
			X2Button,
			Shift = 160,
			Ctrl = 162,
			Alt = 164,
			Capslock = 20,
			Numpad0 = 96,
			Numlock = 144
		}

		private enum Bhopkey
		{
			LeftMouse = 1,
			RightMouse,
			X1Mouse = 5,
			X2Button,
			Shift = 160,
			Ctrl = 162,
			Alt = 164,
			Capslock = 20,
			Numpad0 = 96,
			Numlock = 144
		}

		public enum DeviceCap
		{
			VERTRES = 10,
			DESKTOPVERTRES = 117
		}
		public enum ColorType
		{
			Red,
			Purple
		}


		private void CheckBox2_CheckedChanged(object sender, EventArgs e)
		{
			this.isBhop = this.Bhopbox.Checked;
			this.SetKey("isBhop", this.isBhop);
		}

		private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			this.Bhop = this.Bhopinput.Value;
			this.SetKey("Bhop", this.Bhop);
		}

		private void NumericUpDown1_ValueChanged_1(object sender, EventArgs e)
		{
			this.speed3 = this.Speed3.Value;
			this.SetKey("speed3", this.speed3);
		}

		private void NumericUpDown1_ValueChanged_2(object sender, EventArgs e)
		{
			this.delayx = this.Delayx.Value;
			this.SetKey("delayx", this.delayx);
		}

		private void checkBox2_CheckedChanged_2(object sender, EventArgs e)
		{
			this.isEsp = this.EspBtt.Checked;
			this.SetKey("isEsp", this.isEsp);
		}

		private void CheckBox3_CheckedChanged_1(object sender, EventArgs e)
		{

			if (CircleBtt.Checked == true)
			{
				if (this.isRunning)
				{

				}
			}
			this.isCircle = this.CircleBtt.Checked;
			this.SetKey("isCircle", this.isCircle);
		}


		private void CircleRed_ValueChanged(object sender, EventArgs e)
		{
			this.FovCircleRed = (int)this.CircleRed.Value;
			this.SetKey("FovCircleRed", this.FovCircleRed);
		}

		private void CircleWidth_ValueChanged(object sender, EventArgs e)
		{
			this.FovCircleWidth = (int)this.CircleWidth.Value;
			this.SetKey("FovCircleWidth", this.FovCircleWidth);
		}

		private void CircleGreen_ValueChanged(object sender, EventArgs e)
		{
			this.FovCircleGreen = (int)this.CircleGreen.Value;
			this.SetKey("FovCircleGreen", this.FovCircleGreen);
		}

		private void CircleBlue_ValueChanged(object sender, EventArgs e)
		{
			this.FovCircleBlue = (int)this.CircleBlue.Value;
			this.SetKey("FovCircleBlue", this.FovCircleBlue);
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				Environment.Exit(0);
				this.Close();
			}
			catch { }
		}

		private void Firerage_ValueChanged_3(object sender, EventArgs e)
		{
			if (!isAimbot)
			{
				this.msShootTime = (int)this.Firerage.Value;
				this.SetKey("msShootTime", this.msShootTime);
			}
		}

		private void AimKeyToggle_CheckedChanged(object sender, EventArgs e)
		{
			if (AimKeyToggle.Checked)
			{
				IsHoldToggle.Enabled = true;
			}
			else
			{
				IsHoldToggle.Enabled = false;
				IsHoldToggle.Checked = false;
			}
		}

		public int xSize;

		public int ySize;

		private int msShootTime = 225;

		private DateTime lastShot = DateTime.Now;

		private int offsetY = 10;

		private bool isTriggerbot;

		private bool isAimbot;

		private bool isEsp;

		private bool TriggerRage;

		public bool isCircle;

		private bool isRecoil;

		private bool isBhop;

		private decimal PingX = 100;

		private decimal speed = 1m;

		private decimal speed3 = 1m;

		private decimal Bhop = 4;

		private decimal delayx = 10000;

		public int fovX;

		public int fovY;

		public int FovCircleRed = 255;

		public int FovCircleGreen = 255;

		public int FovCircleBlue = 255;

		public int FovCircleWidth = 2;

		private bool isAimKey;

		private bool isHold = true;

		private int monitor;

		private int colorVariation = 50;

		private Form1.AimKey mainAimKey = Form1.AimKey.Alt;

		private Form1.Bhopkey Bhopxkey = Form1.Bhopkey.Alt;

		private Form1.ColorType color = Form1.ColorType.Purple;

		private float zoom = 1f;

		private Thread mainThread;

		private bool isRunning;

		private bool slowmove;

		private int antirecoilval;

		private void ColX_ValueChanged(object sender, EventArgs e)
		{

		}

		private void ColY_ValueChanged(object sender, EventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{

		}

		private void guna2Button1_Click(object sender, EventArgs e)
		{

		}

		private void guna2Button3_Click(object sender, EventArgs e)
		{

		}

		private void label36_Click(object sender, EventArgs e)
		{

		}

		private void label1_Click(object sender, EventArgs e)
		{

		}
	}

	public struct MoveInfo
	{
		public int xAmount;
		public int yAmount;
		public uint MovePack;
		public InternCaseMoveSettings MovementSettings;
		public uint Waittimems;
		public IntPtr Information;
	}

	public enum InternCaseMoveSettings
	{
		LeftDown = 2,
		LeftUp = 4,
		Move = 1,
		MoveNoCoalesce = 8192
	}

	public static class CaseExecute
	{
		public static void ExecuteMovementCase(MoveInfo input)
		{
			CaseExecute.ExecuteMovementCase(new MoveInfo[]
			{
				input
			});
		}

		public static void ExecuteMovementCase(MoveInfo[] inputs)
		{
			if (!Execute.InjectMouseInput(inputs, inputs.Length))
			{
				throw new Win32Exception();
			}
		}
	}
	public static class Execute
	{
		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool InjectMouseInput([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] MoveInfo[] inputs, int count);
	}
}

