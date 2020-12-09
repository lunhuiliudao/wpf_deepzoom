using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Shazzam.Shaders
{
	public class ImgfiterEffect : ShaderEffect
	{
		public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(ImgfiterEffect), 0);

		public static readonly DependencyProperty BrightnessProperty = DependencyProperty.Register("Brightness", typeof(double), typeof(ImgfiterEffect), new UIPropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(0)));

		public static readonly DependencyProperty ContrastProperty = DependencyProperty.Register("Contrast", typeof(double), typeof(ImgfiterEffect), new UIPropertyMetadata(1.5, ShaderEffect.PixelShaderConstantCallback(1)));

		public static readonly DependencyProperty RProperty = DependencyProperty.Register("R", typeof(double), typeof(ImgfiterEffect), new UIPropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(2)));

		public static readonly DependencyProperty GProperty = DependencyProperty.Register("G", typeof(double), typeof(ImgfiterEffect), new UIPropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(3)));

		public static readonly DependencyProperty BProperty = DependencyProperty.Register("B", typeof(double), typeof(ImgfiterEffect), new UIPropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(4)));

		public static readonly DependencyProperty AmountProperty = DependencyProperty.Register("Amount", typeof(double), typeof(ImgfiterEffect), new UIPropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(5)));

		public static readonly DependencyProperty InputSizeProperty = DependencyProperty.Register("InputSize", typeof(Point), typeof(ImgfiterEffect), new UIPropertyMetadata(new Point(0.0, 0.0), ShaderEffect.PixelShaderConstantCallback(6)));

		public static readonly DependencyProperty GamaProperty = DependencyProperty.Register("Gama", typeof(double), typeof(ImgfiterEffect), new UIPropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(7)));

		public Brush Input
		{
			get
			{
				return (Brush)GetValue(InputProperty);
			}
			set
			{
				SetValue(InputProperty, value);
			}
		}

		public double Brightness
		{
			get
			{
				return (double)GetValue(BrightnessProperty);
			}
			set
			{
				SetValue(BrightnessProperty, value);
			}
		}

		public double Contrast
		{
			get
			{
				return (double)GetValue(ContrastProperty);
			}
			set
			{
				SetValue(ContrastProperty, value);
			}
		}

		public double R
		{
			get
			{
				return (double)GetValue(RProperty);
			}
			set
			{
				SetValue(RProperty, value);
			}
		}

		public double G
		{
			get
			{
				return (double)GetValue(GProperty);
			}
			set
			{
				SetValue(GProperty, value);
			}
		}

		public double B
		{
			get
			{
				return (double)GetValue(BProperty);
			}
			set
			{
				SetValue(BProperty, value);
			}
		}

		public double Amount
		{
			get
			{
				return (double)GetValue(AmountProperty);
			}
			set
			{
				SetValue(AmountProperty, value);
			}
		}

		public Point InputSize
		{
			get
			{
				return (Point)GetValue(InputSizeProperty);
			}
			set
			{
				SetValue(InputSizeProperty, value);
			}
		}

		public double Gama
		{
			get
			{
				return (double)GetValue(GamaProperty);
			}
			set
			{
				SetValue(GamaProperty, value);
			}
		}

		public ImgfiterEffect()
		{
			base.PixelShader = new PixelShader
			{
				UriSource = new Uri("/KFBIO.SlideViewer;component/shade/imgfiter.ps", UriKind.Relative)
			};
			UpdateShaderValue(InputProperty);
			UpdateShaderValue(BrightnessProperty);
			UpdateShaderValue(ContrastProperty);
			UpdateShaderValue(RProperty);
			UpdateShaderValue(GProperty);
			UpdateShaderValue(BProperty);
			UpdateShaderValue(AmountProperty);
			UpdateShaderValue(InputSizeProperty);
			UpdateShaderValue(GamaProperty);
		}
	}
}
