using System;
using System.Collections.Generic;
using System.Text;

namespace SmartX.Components
{
    /// <summary>
    /// 组件属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentAttribute : Attribute
    {
        /// <summary>
        /// 生命周期
        /// </summary>
        public LifeStyle LifeStyle { get; private set; }

        public ComponentAttribute() : this(LifeStyle.Singleton) { }

        public ComponentAttribute(LifeStyle lifeStyle)
        {
            LifeStyle = lifeStyle;
        }
    }

    public enum LifeStyle
    {
        /// <summary>
        /// 每次都重新创建一个实例
        /// </summary>
        Transient,

        /// <summary>
        /// 创建一个单例
        /// </summary>
        Singleton
    }
}
