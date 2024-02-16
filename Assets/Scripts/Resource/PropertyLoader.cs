// 写了一个玩具配置文件解析器，练习用

using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class PropertyLoader
{
    private StringReader stringReader;
    private Dictionary<string, string> props; 
    
    /// <summary>
    /// stringReader 为文件内容流
    /// </summary>
    public PropertyLoader(StringReader stringReader)
    {
        this.stringReader = stringReader;
        props = new Dictionary<string, string>();
    }

    public string GetProperty(string key)
    {
        if (props.ContainsKey(key))
        {
            return props[key];
        }
        
        // 用正则获取每一行的 key 和 value
        string line;
        string pattern = @"^\s*(?<key>\w+)\s*=\s*(?<value>.*)$";
        while ((line = stringReader.ReadLine()) != null)
        {
            var match = Regex.Match(line, pattern);
            if (!match.Success)
            {
                Debug.LogError($"非法的属性行：{line}");
                return null;
            }
            else
            {
                string k = match.Groups["key"].Value.Trim();
                string v = match.Groups["value"].Value.Trim();
                props[k] = v;
                if (k == key)
                {
                    return v;
                }
            }
        }
        Debug.LogError($"没有找到属性：{key}");
        return null;
    }
}