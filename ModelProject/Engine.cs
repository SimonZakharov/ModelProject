using System;

namespace ModelProject
{
    abstract class Engine
    {
        #region Поля класса
        /// <summary>
        /// Скорость двигателя.
        /// </summary>
        public double Velocity { get; protected set; }
        /// <summary>
        /// Ускорение двигателя.
        /// </summary>
        public double Acceleration { get; protected set; }
        /// <summary>
        /// Момент инерции, кг*м2.
        /// </summary>
        public double I { get; protected set; }
        /// <summary>
        /// Температура двигателя.
        /// </summary>
        public double Temperature { get; protected set; }
        /// <summary>
        /// Температура перегрева двигателя.
        /// </summary>
        public double TOverheat { get; protected set; }
        #endregion
        #region Абстрактные методы, требующие реализации в производных классах
        public virtual void SetVelocity(double param) { Velocity = param; }
        public virtual void SetAcceleration(double param) { Acceleration = param; }
        public virtual void SetTOverheat(double param) { TOverheat = param; }
        public abstract Tuple<int, double> StartEngine(double outerTemperature, string target);
        #endregion
    }
}
