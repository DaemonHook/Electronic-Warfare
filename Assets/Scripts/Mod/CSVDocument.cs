/*
 * file: CSVDocument.cs
 * author: D.H.
 * feature: CSV文件读取
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// CSV配置文件读取
/// </summary>
public class CSVDocument
{
    public int Column { get; private set; }
    public List<string> Headers { get; private set; }
    public List<Dictionary<string, string>> Data { get; private set; }

    public CSVDocument(string text)
    {
        var lines = text.Split("\n");
        if (lines.Length == 0) return;
        Headers = new List<string>(from word in lines[0].Split(",")
            select word.Trim());
        Column = Headers.Count;
        Data = new List<Dictionary<string, string>>();
        for (int i = 1; i < lines.Length; i++)
        {
            var lineDic = new Dictionary<string, string>();
            var words = new List<string>(from word in lines[i].Split(",")
                select word.Trim());
            if (words.Count == Column)
                for (int j = 0; j < Column; j++)
                {
                    lineDic.Add(Headers[j], words[j]);
                }

            Data.Add(lineDic);
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("CSVDocument:\n");

        foreach (var colName in Headers)
        {
            sb.Append(colName + '\t');
        }

        sb.Append('\n');

        foreach (var line in Data)
        {
            foreach (var item in line.Values)
            {
                sb.Append(item + '\t');
            }

            sb.Append('\n');
        }

        return sb.ToString();
    }
}