﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Ninject;
using Ninject.Extensions.Factory;
using PCG;
using Ninject.Parameters;
using System.Reflection;
using System.Runtime.CompilerServices;
using Ninject.Infrastructure.Language;

namespace PCGterrain
{
	public interface IGeneratorFactory
	{
		HeightMapGenerator CreateGenerator(string name);
	}

	public class UseFirstArgumentAsNameInstanceProvider : StandardInstanceProvider
	{
		protected override string GetName(System.Reflection.MethodInfo methodInfo, 
			object[] arguments)
		{
			return (string)arguments[0];
		}
	}

	class MenuForm : Form
	{
		static StandardKernel container;
		private static Dictionary<string, List<string>> generatorsNamesParameters;

		static MenuForm()
		{
			var contein = new StandardKernel();
			contein.Bind<HeightMapGenerator>()
			.To<BilinearValueInterpolationGenerator>()
			.Named("BilinearValueInterpolationGenerator");
			contein.Bind<HeightMapGenerator>()
			.To<NonlinearValueInterpolationGenerator>()
			.Named("NonlinearValueInterpolationGenerator");
			contein.Bind<HeightMapGenerator>()
			.To<CosineValueInterpolationGenerator>()
			.Named("CosineValueInterpolationGenerator");
			contein.Bind<HeightMapGenerator>()
			.To<PerlinGradientInterpolationGenerator>()
			.Named("PerlinGradientInterpolationGenerator");
			contein.Bind<HeightMapGenerator>()
			.To<HillGenerator>()
			.Named("HillGenerator");
			contein.Bind<HeightMapGenerator>()
			.To<DiamondSquareGenerator>()
			.Named("DiamondSquareGenerator");
			contein.Bind<IGeneratorFactory>()
				.ToFactory(() => new UseFirstArgumentAsNameInstanceProvider());
			container = contein;
			Type ourtype = typeof(HeightMapGenerator);
			var
				dictionary1 = Assembly
				.GetAssembly(ourtype)
				.GetTypes()
				.Where(type => ourtype.IsAssignableFrom(type) && !type.IsAbstract)
				;
			var d= dictionary1
				
				.ToDictionary(t=>t.Name,t=> {
					var parameters =
					t.GetFields()
					.Select(p=>p.Name)
					.ToList();
					return parameters;
				});
			generatorsNamesParameters = d;
			foreach (var p in d)
			{
				Console.WriteLine(p.Key.ToString());
				foreach (var v in p.Value)
					Console.WriteLine(v);
			}
				
		}

		public void OnChange(Label label, ListBox listBox)
		{
			label.Text = listBox.SelectedItem.ToString();
			var choise = label.Text;
			var parameters = generatorsNamesParameters[choise];
			var startLocation = new Point(50, listBox.Bottom + 10);
			var numericList = new List<NumericUpDown>();
			var labelList = new List<Label>();
			foreach (string p in (new List<string>() { "width", "height", 
				"times to run to get statistics" })
			.Concat(parameters))
			{
				var lab = new Label
				{
					Location = startLocation,
					Size = new Size(400, 20),
					Text = p,
					Font = new Font("Times New Roman", 14, FontStyle.Regular)
				};
				labelList.Add(lab);
				startLocation = new Point(50, startLocation.Y + 25);
				var el = new NumericUpDown
				{
					Location = startLocation,
					Size = new Size(100, 40),
					Font = new Font("Times New Roman", 14, FontStyle.Regular),
					Maximum = 100000,
					Minimum = 1
					
				};
				numericList.Add(el);
				startLocation = new Point(50, startLocation.Y + 45);
			}
			Controls.AddRange(numericList.ToArray());
			Controls.AddRange(labelList.ToArray());
			var button = new Button
			{
				Location = startLocation,
				Size = new Size(500, 50),
				Font = new Font("Times New Roman", 14, FontStyle.Regular),
				Text = "GENERATE",
				BackColor = Color.LavenderBlush
			};
			
			button.Click += (sender1, args1) =>
			{
				var fbd = new FolderBrowserDialog();
				fbd.Description =
				"Select the directory where you wanr to save picture";
				fbd.ShowNewFolderButton = false;
				if (fbd.ShowDialog() == DialogResult.OK)
				{
					Console.WriteLine(fbd.SelectedPath);
					OnClick(labelList, numericList, 
						choise, fbd.SelectedPath);
				}
			};
			Controls.Add(button);
		}

		public void GetOnlyLabel(string text)
		{
			var lab = new Label
			{
				Location = new Point(0, 0),
				Size = new Size(ClientSize.Width, ClientSize.Height),
				Text = text,
				TextAlign = ContentAlignment.MiddleCenter,
				Font = new Font("Times New Roman", 24, FontStyle.Regular)
			};
			Controls.Clear();
			Controls.Add(lab);
		}

		public void OnClick(List<Label> labelList, List<NumericUpDown> numericList,
			string choise, string path)
		{
			var labesText = labelList.Skip(3).Select(lab1 => lab1.Text);
			var list = numericList.Skip(3).Select(el => (int)el.Value).ToList();
			var resD = labesText.Zip(list, (w1, w2) => (w1, w2))
			.ToDictionary(p => p.w1, p => p.w2);
			var names = generatorsNamesParameters[choise];
			var f = container.Get<IGeneratorFactory>();
			try
			{
				var width = (int)(numericList[0].Value);
				var height = (int)numericList[1].Value;
				var n = (int)numericList[2].Value;
				var name = choise;
				var gen = f.CreateGenerator(name);
				var prop = gen
				.GetType()
				.GetFields()
				.Where(fld => names.Contains(fld.Name));
				foreach (var pr in prop)
					pr.SetValue(gen, resD[pr.Name]);
				Program.GenerateHeightMapPic(gen, width, height, false, path);
				var gs = new GenerationStatistics();
				var res = gs.GetStatistics(gen, n, width, height);
				GetOnlyLabel("STATISTICS\n" + res);
				//this.Close();
			}
			catch (ArgumentException e)
			{
				GetOnlyLabel("ERROR\n" + e.Message.ToString());
			}
		}

		public MenuForm()
		{
			var label = new Label
			{
				Location = new Point(50, 10),
				Size = new Size(500, 30),
				TextAlign = ContentAlignment.MiddleCenter,
				Text = "Choose type of generation",
				Font = new Font("Times New Roman", 14, FontStyle.Regular),
				BackColor = Color.LavenderBlush,
				BorderStyle = BorderStyle.FixedSingle
			};
			var listBox = new ListBox
			{
				Location = new Point(50, label.Bottom+20),
				Size = new Size(500, 80),
				Font = new Font("Times New Roman", 14, FontStyle.Regular)
			};
			listBox.Items.AddRange(generatorsNamesParameters.Keys.ToArray());
			listBox.SelectedIndexChanged += (sender, args) =>
			{
				OnChange(label, listBox);
			};
			Controls.Add(label);
			Controls.Add(listBox);
		}		
	}
}