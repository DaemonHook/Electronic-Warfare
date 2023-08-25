using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class BaseView: MonoBehaviour
{
    public int Id { get; set; }
    public virtual void OnOpen()
    {

    }
    
    public virtual void OnClose()
    {

    }
}