/*
 * file: EventType.cs
 * author: oldball
 * feature: 事件类型之枚举
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 事件类型的枚举，为位标志
/// </summary>
public enum EventType
{
    move = 0b1,
    damage = 0b10
}