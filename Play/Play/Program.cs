using System.Text.Json.Serialization;
using System.Text.Json;
using System.Reflection;
using System.Text;

namespace Play;


static class Program
{
	static void Main()
	{
		Console.WriteLine("\nBegin LSTM IO demo \n");

		Console.WriteLine("Creating an n=2 input, m=3 state LSTM cell");
		Console.WriteLine("Setting LSTM weights and biases to small arbitrary values \n");
		Console.WriteLine("Sending input = (1.0, 2.0) to LSTM \n");

		var xt = MatFromArray([1.0f, 2.0f], 2, 1);
		var h_prev = MatFromArray([0.0f, 0.0f, 0.0f], 3, 1);
		var c_prev = MatFromArray([0.0f, 0.0f, 0.0f], 3, 1);

		var W = MatFromArray([
			0.01f, 0.02f,
			0.03f, 0.04f,
			0.05f, 0.06f,
		], 3, 2);

		var U = MatFromArray([
			0.07f, 0.08f, 0.09f,
			0.10f, 0.11f, 0.12f,
			0.13f, 0.14f, 0.15f,
		], 3, 3);

		var b = MatFromArray([0.16f, 0.17f, 0.18f], 3, 1);

		var Wf = MatCopy(W); var Wi = MatCopy(W);
		var Wo = MatCopy(W); var Wc = MatCopy(W);

		var Uf = MatCopy(U); var Ui = MatCopy(U);
		var Uo = MatCopy(U); var Uc = MatCopy(U);

		var bf = MatCopy(b); var bi = MatCopy(b);
		var bo = MatCopy(b); var bc = MatCopy(b);

		var result = ComputeOutputs(xt, h_prev, c_prev, Wf, Wi, Wo, Wc, Uf, Ui, Uo, Uc, bf, bi, bo, bc);

		var ht = result[0];  // output
		var ct = result[1];  // new cell state

		Console.WriteLine("Output is:");
		MatPrint(ht, 4, true);
		Console.WriteLine("New cell state is:");
		MatPrint(ct, 4, true);

		Console.WriteLine("=====");
		Console.WriteLine("\nSending input = (3.0, 4.0) to LSTM \n");

		h_prev = MatCopy(ht);
		c_prev = MatCopy(ct);
		xt = MatFromArray([3.0f, 4.0f], 2, 1);

		result = ComputeOutputs(xt, h_prev, c_prev, Wf, Wi, Wo, Wc, Uf, Ui, Uo, Uc, bf, bi, bo, bc);

		ht = result[0];
		ct = result[1];

		Console.WriteLine("Output is:");
		MatPrint(ht, 4, true);
		Console.WriteLine("New cell state is:");
		MatPrint(ct, 4, true);

		Console.WriteLine("End LSTM demo ");
	}



	static float[][][] ComputeOutputs(float[][] xt, float[][] h_prev, float[][] c_prev,
		  float[][] Wf, float[][] Wi, float[][] Wo, float[][] Wc,
		  float[][] Uf, float[][] Ui, float[][] Uo, float[][] Uc,
		  float[][] bf, float[][] bi, float[][] bo, float[][] bc)
	{
		var ft = MatSig(MatSum(MatProd(Wf, xt), MatProd(Uf, h_prev), bf));
		var it = MatSig(MatSum(MatProd(Wi, xt), MatProd(Ui, h_prev), bi));
		var ot = MatSig(MatSum(MatProd(Wo, xt), MatProd(Uo, h_prev), bo));
		var ct = MatSum(MatHada(ft, c_prev),
		  MatHada(it, MatTanh(MatSum(MatProd(Wc, xt), MatProd(Uc, h_prev), bc))));
		var ht = MatHada(ot, MatTanh(ct));

		float[][][] result = [MatCopy(ht), MatCopy(ct)];
		return result;
	}

	// Matrix routines

	static float[][] MatCreate(int rows, int cols)
	{
		var result = new float[rows][];
		for (var i = 0; i < rows; ++i)
			result[i] = new float[cols];
		return result;
	}
	static float[][] MatFromArray(float[] arr, int rows, int cols)
	{
		if (rows * cols != arr.Length)
			throw new Exception("xxx");

		var result = MatCreate(rows, cols);
		var k = 0;
		for (var i = 0; i < rows; ++i)
			for (var j = 0; j < cols; ++j)
				result[i][j] = arr[k++];
		return result;
	}

	static float[][] MatCopy(float[][] m)
	{
		var rows = m.Length; var cols = m[0].Length;
		var result = MatCreate(rows, cols);
		for (var i = 0; i < rows; ++i)
			for (var j = 0; j < cols; ++j)
				result[i][j] = m[i][j];
		return result;
	}
	static float[][] MatProd(float[][] a, float[][] b)
	{
		var aRows = a.Length; var aCols = a[0].Length;
		var bRows = b.Length; var bCols = b[0].Length;
		if (aCols != bRows)
			throw new Exception("xxx");
		var result = MatCreate(aRows, bCols);
		for (var i = 0; i < aRows; ++i) // each row of a
			for (var j = 0; j < bCols; ++j) // each col of b
				for (var k = 0; k < aCols; ++k) // could use k < bRows
					result[i][j] += a[i][k] * b[k][j];
		return result;
	}

	// element-wise functions

	static float[][] MatSig(float[][] m)
	{
		// element-wise sigmoid
		var rows = m.Length; var cols = m[0].Length;

		var result = MatCreate(rows, cols);
		for (var i = 0; i < rows; ++i) // each row
			for (var j = 0; j < cols; ++j) // each col
				result[i][j] = Sigmoid(m[i][j]);
		return result;
	}

	static float[][] MatTanh(float[][] m)
	{
		// element-wise tanh
		var rows = m.Length; var cols = m[0].Length;

		var result = MatCreate(rows, cols);
		for (var i = 0; i < rows; ++i) // each row
			for (var j = 0; j < cols; ++j) // each col
				result[i][j] = Tanh(m[i][j]);
		return result;
	}

	static float Sigmoid(float x)
	{
		if (x < -10.0) return 0.0f;
		else if (x > 10.0) return 1.0f;
		return (float)(1.0 / (1.0 + Math.Exp(-x)));
	}

	static float Tanh(float x)
	{
		if (x < -10.0) return -1.0f;
		else if (x > 10.0) return 1.0f;
		return (float)(Math.Tanh(x));
	}
	static float[][] MatHada(float[][] a, float[][] b)
	{
		// Hadamard element-wise multiplication
		// assumes a, b have same shape
		var rows = a.Length; var cols = a[0].Length;

		var result = MatCreate(rows, cols);
		for (var i = 0; i < rows; ++i)
			for (var j = 0; j < cols; ++j)
				result[i][j] = a[i][j] * b[i][j];
		return result;
	}

	static float[][] MatSum(float[][] a, float[][] b)
	{
		var rows = a.Length; var cols = a[0].Length;

		var result = MatCreate(rows, cols);
		for (var i = 0; i < rows; ++i)
			for (var j = 0; j < cols; ++j)
				result[i][j] = a[i][j] + b[i][j];
		return result;
	}

	static float[][] MatSum(float[][] a, float[][] b, float[][] c)
	{
		var rows = a.Length; var cols = a[0].Length;

		var result = MatCreate(rows, cols);
		for (var i = 0; i < rows; ++i)
			for (var j = 0; j < cols; ++j)
				result[i][j] = a[i][j] + b[i][j] + c[i][j];
		return result;
	}
	static void MatPrint(float[][] Mat, int dec, bool nl)
	{
		for (var i = 0; i < Mat.Length; ++i)
		{
			for (var j = 0; j < Mat[0].Length; ++j)
			{
				Console.Write(Mat[i][j].ToString("F" + dec) + " ");
			}
			Console.WriteLine("");
		}
		if (nl == true) Console.WriteLine("");
	}
}
