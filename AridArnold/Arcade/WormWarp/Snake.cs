using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WormWarp
{
	enum SDirection
	{
		Up,
		Down,
		Left,
		Right
	}

	struct WarpPortal
	{
		public Vector2 In;
		public Vector2 Out;

		public WarpPortal(Vector2 i, Vector2 o)
		{
			In = i;
			Out = o;
		}
	}

	class Snake
	{


		public List<Vector2> Positions = new List<Vector2>();

		List<WarpPortal> WarpPoints = new List<WarpPortal>();

		SDirection prevDir;
		SDirection currDir;
		Texture2D HeadTexture;
		Texture2D BodyTexture;
		Texture2D CornerTexture;
		Texture2D TailTexture;

		public Snake(Vector2 StartPos, ref Texture2D HT, ref Texture2D BT, ref Texture2D CT, ref Texture2D TT)
		{
			HeadTexture = HT;
			BodyTexture = BT;
			CornerTexture = CT;
			TailTexture = TT;
			prevDir = SDirection.Down;
			currDir = SDirection.Left;
			Positions.Add(StartPos);
			Positions.Add(StartPos + new Vector2(0, -1));
			Positions.Add(StartPos + new Vector2(0, -2));
			Positions.Add(StartPos + new Vector2(0, -3));
		}

		public void Draw(SpriteBatch spriteBatch, Camera Cam, Vector2 Offset, int blockw, int blockh)
		{
			for (int i = 0; i < Positions.Count(); i++)
			{
				Vector2 Pos = new Vector2(Positions[i].X - Cam.X, Positions[i].Y + Cam.Y);
				float rotation = 0.0f;

				Texture2D toDraw;
				SpriteEffects efx = SpriteEffects.None;

				if (i == 0) //Head
				{
					toDraw = HeadTexture;
					rotation = angleOfDir(prevDir) + (float)Math.PI;
				}
				else if (i == Positions.Count() - 1) //Tail
				{
					Vector2 Dir = Pos - Positions[i - 1];

					for (int j = 0; j < WarpPoints.Count(); j++)
					{
						if (Utilities.OrthogonallyAdj(Pos, WarpPoints[j].In))
						{
							Dir = Pos - WarpPoints[j].In;
							break;
						}
					}

					rotation = Utilities.VectAngle(Dir) + (float)Math.PI;
					toDraw = TailTexture;
				}
				else
				{
					toDraw = BodyTexture;

					Vector2 Behind = Positions[i + 1] - Pos;

					for (int j = 0; j < WarpPoints.Count(); j++)
					{
						if (Utilities.OrthogonallyAdj(Pos, WarpPoints[j].Out))
						{
							Behind = WarpPoints[j].Out - Pos;
							break;
						}
					}

					Vector2 Front = Positions[i - 1] - Pos;

					for (int j = 0; j < WarpPoints.Count(); j++)
					{
						if (Utilities.OrthogonallyAdj(Pos, WarpPoints[j].In))
						{
							Front = WarpPoints[j].In - Pos;
							break;
						}
					}

					if (Front.X == 0 && Behind.X == 0)//Going Vertical
					{
						if (Front.Y == 1)//Going down
						{
							rotation = (float)(0.5f * Math.PI);
						}
						else//Going up
						{
							rotation = (float)(1.5f * Math.PI);
						}
					}
					else if (Front.Y == 0 && Behind.Y == 0)//Going Horizontal
					{
						if (Front.X == 1)//Going right
						{
							rotation = (0.0f);
						}
						else//Going left
						{
							rotation = (float)(Math.PI);
						}
					}
					else
					{
						if (Front.Y == -1 && Behind.X == -1)// Top n Left
						{
							rotation = 0.0f;
						}
						else if (Front.X == -1 && Behind.Y == -1)
						{
							rotation = (float)(0.5f * Math.PI);
							efx = SpriteEffects.FlipVertically;
						}
						else if (Front.X == 1 && Behind.Y == -1) // Top n Right
						{
							rotation = (float)(0.5f * Math.PI);
						}
						else if (Front.Y == -1 && Behind.X == 1) // Top n Right
						{
							rotation = 0.0f;
							efx = SpriteEffects.FlipHorizontally;
						}
						else if (Front.Y == 1 && Behind.X == -1)// Bottom n Left
						{
							rotation = 0.0f;
							efx = SpriteEffects.FlipVertically;
						}
						else if (Front.X == -1 && Behind.Y == 1)// Bottom n Left
						{
							rotation = (float)(1.5f * Math.PI);
							//efx = SpriteEffects.FlipVertically;
						}
						else if (Front.X == 1 && Behind.Y == 1) // Bottom n Right
						{
							rotation = (float)(0.5f * Math.PI);
							efx = SpriteEffects.FlipHorizontally;
						}
						else if (Front.Y == 1 && Behind.X == 1) // Bottom n Right
						{
							rotation = (float)(Math.PI);
						}

						toDraw = CornerTexture;
					}
				}

				Rectangle bounds = Utilities.getBounds(Offset, (int)Pos.X, (int)Pos.Y, blockw, blockh);

				Utilities.DrawWithRotation(spriteBatch, toDraw, bounds, rotation, efx);
			}
		}

		public void Move()
		{
			for (int i = Positions.Count - 1; i > 0; i--)
			{
				if (i == Positions.Count - 1)
				{
					for (int j = 0; j < WarpPoints.Count(); j++)
					{
						if (Utilities.OrthogonallyAdj(Positions[i], WarpPoints[j].Out))
						{
							WarpPoints.RemoveAt(j);
							break;
						}
					}
				}
				Positions[i] = Positions[i - 1];
			}

			Positions[0] += VecFromDir(currDir);

			HandleEdge();

			prevDir = currDir;
		}

		public void SetNewDir(SDirection new_dir)
		{
			Vector2 potential_pos = Positions[0] + VecFromDir(new_dir);
			if (!IsSpaceAvailable(Positions[0] + VecFromDir(new_dir), false))
			{
				return;
			}

			for (int i = 0; i < WarpPoints.Count(); i++)
			{
				if (WarpPoints[i].Out == potential_pos)
				{
					return;
				}
			}

			currDir = new_dir;
		}

		void HandleEdge()
		{
			int offset = -1;
			int curr_edge = -1;
			if (Positions[0].X < 0) // Going off the left
			{
				offset = SnakeGame.THEIGHT - 1 - (int)Positions[0].Y;
				curr_edge = 3;
			}
			else if (Positions[0].X > SnakeGame.TWIDTH - 1) // Going off the right
			{
				offset = (int)Positions[0].Y;
				curr_edge = 1;
			}
			else if (Positions[0].Y < 0) // Going off the top
			{
				offset = (int)Positions[0].X;
				curr_edge = 0;
			}
			else if (Positions[0].Y > SnakeGame.THEIGHT - 1) // Going off the bottom
			{
				offset = SnakeGame.TWIDTH - 1 - (int)Positions[0].X;
				curr_edge = 2;
			}

			if (offset > -1)
			{
				List<int> edges = new List<int>(new[] { 0, 1, 2, 3 });
				edges.Remove(curr_edge);

				Vector2 out_pos;
				int NewEdge = 0;
				do
				{
					NewEdge = edges[Utilities.RNG.Next(0, edges.Count)];
					out_pos = GenerateEdgePos(offset, NewEdge);
					edges.Remove(NewEdge);

				} while (!ValidEdge(out_pos) && edges.Count > 0);

				SDirection new_direction = DirFromEdge(NewEdge);
				currDir = new_direction;
				prevDir = new_direction;

				WarpPoints.Add(new WarpPortal(Positions[0], out_pos - VecFromDir(new_direction)));
				Positions[0] = out_pos;


			}
		}

		bool ValidEdge(Vector2 pos)
		{
			return IsSpaceAvailable(pos);
		}

		Vector2 GenerateEdgePos(int offset, int edge)
		{
			switch (edge)
			{
				case 0: //Top
					return new Vector2(offset, 0);
				case 1: //Right
					return new Vector2(SnakeGame.TWIDTH - 1, offset);
				case 2: //Down
					return new Vector2(SnakeGame.TWIDTH - 1 - offset, SnakeGame.THEIGHT - 1);
				default: //Left
					return new Vector2(0, SnakeGame.THEIGHT - 1 - offset);
			}
		}

		SDirection DirFromEdge(int edge)
		{
			switch (edge)
			{
				case 0: //Top
					return SDirection.Down;
				case 1: //Right
					return SDirection.Left;
				case 2: //Down
					return SDirection.Up;
				default: //Left
					return SDirection.Right;
			}
		}

		public bool IsSpaceAvailable(Vector2 a, bool allow_last = true)
		{
			int size = Positions.Count;
			if (allow_last == false)
			{
				size--;
			}

			for (int i = 0; i < size; i++)
			{
				if (Positions[i] == a)
				{
					return false;
				}
			}

			return true;
		}

		public bool IsEatingItself()
		{
			for (int i = 1; i < Positions.Count; i++)
			{
				if (Positions[i] == Positions[0])
				{
					return true;
				}
			}

			return false;
		}

		bool SpaceOffTheBoard(Vector2 a)
		{
			if (a.X < 0 || a.X > SnakeGame.TWIDTH - 1 || a.Y < 0 || a.Y > SnakeGame.THEIGHT - 1)
			{
				return true;
			}

			return false;
		}

		public Vector2 VecFromDir(SDirection dir)
		{
			switch (dir)
			{
				case SDirection.Up:
					return new Vector2(0, -1);
				case SDirection.Down:
					return new Vector2(0, 1);
				case SDirection.Left:
					return new Vector2(-1, 0);
				default:
					return new Vector2(1, 0);
			}
		}

		public float angleOfDir(SDirection dir)
		{
			switch (dir)
			{
				case SDirection.Up:
					return (float)(Math.PI * 0.5f);
				case SDirection.Down:
					return (float)(Math.PI * 1.5f);
				case SDirection.Left:
					return 0.0f;
				default:
					return (float)(Math.PI * 1.0f);
			}
		}

	}
}
