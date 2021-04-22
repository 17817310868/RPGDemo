using System;
using UnityEngine;

namespace Net
{
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
    [Serializable]
    public struct Color32
	{
		// Token: 0x06003F1C RID: 16156 RVA: 0x000720FE File Offset: 0x000702FE
		public Color32(byte r, byte g, byte b, byte a)
		{
			this.rgba = 0;
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		// Token: 0x06003F1D RID: 16157 RVA: 0x00072128 File Offset: 0x00070328
		public static implicit operator Color32(Color c)
		{
			return new Color32((byte)(Mathf.Clamp01(c.r) * 255f), (byte)(Mathf.Clamp01(c.g) * 255f), (byte)(Mathf.Clamp01(c.b) * 255f), (byte)(Mathf.Clamp01(c.a) * 255f));
		}

		// Token: 0x06003F1E RID: 16158 RVA: 0x00072190 File Offset: 0x00070390
		public static implicit operator Color(Color32 c)
		{
			return new Color((float)c.r / 255f, (float)c.g / 255f, (float)c.b / 255f, (float)c.a / 255f);
		}

		// Token: 0x06003F1F RID: 16159 RVA: 0x000721E4 File Offset: 0x000703E4
		public static Color32 Lerp(Color32 a, Color32 b, float t)
		{
			t = Mathf.Clamp01(t);
			return new Color32((byte)((float)a.r + (float)(b.r - a.r) * t), (byte)((float)a.g + (float)(b.g - a.g) * t), (byte)((float)a.b + (float)(b.b - a.b) * t), (byte)((float)a.a + (float)(b.a - a.a) * t));
		}

		// Token: 0x06003F20 RID: 16160 RVA: 0x00072278 File Offset: 0x00070478
		public static Color32 LerpUnclamped(Color32 a, Color32 b, float t)
		{
			return new Color32((byte)((float)a.r + (float)(b.r - a.r) * t), (byte)((float)a.g + (float)(b.g - a.g) * t), (byte)((float)a.b + (float)(b.b - a.b) * t), (byte)((float)a.a + (float)(b.a - a.a) * t));
		}

		// Token: 0x06003F21 RID: 16161 RVA: 0x00072304 File Offset: 0x00070504
		public override string ToString()
		{
			return string.Format("RGBA({0}, {1}, {2}, {3})", new object[]
			{
				this.r,
				this.g,
				this.b,
				this.a
			});
		}

		// Token: 0x06003F22 RID: 16162 RVA: 0x00072364 File Offset: 0x00070564
		public string ToString(string format)
		{
			return string.Format("RGBA({0}, {1}, {2}, {3})", new object[]
			{
				this.r.ToString(format),
				this.g.ToString(format),
				this.b.ToString(format),
				this.a.ToString(format)
			});
		}

		private int rgba;

		public byte r;

		public byte g;

		public byte b;

		public byte a;
	}
}
