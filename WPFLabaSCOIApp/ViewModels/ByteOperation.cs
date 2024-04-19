using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFLabaSCOIApp.ViewModels
{
    public class ByteOperation
    {
        public delegate byte Operation(byte oldByte, byte newByte, double opacity, bool flag);
        public Operation SelectedOperation { get; set; }
        public string Name { get; set; }

        private static List<ByteOperation> _operationsList;
        public static List<ByteOperation> getOperationsList()
        {
            return _operationsList ??= new List<ByteOperation>()
            {
                new ByteOperation()
                {
                    Name = "Обычное",
                    SelectedOperation = (oldByte, newByte, opacity, flag) =>
                    {
                        if (flag)
                            return (byte)(oldByte * (1 - opacity) + newByte * opacity);
                        else
                            return (byte)(oldByte * (1 - opacity));
                    }
                },

                new ByteOperation()
                {
                    Name = "Умножение",
                    SelectedOperation = (oldByte, newByte, opacity, flag) =>
                    {
                        if (flag)
                            return (byte)((oldByte * newByte * opacity)/255);
                        else
                            return (byte)(oldByte);
                    }
                },

                new ByteOperation()
                {
                    Name = "Деление",
                    SelectedOperation = (oldByte, newByte, opacity, flag) =>
                    {
                        if (flag)
                            return (byte)(oldByte/(newByte * opacity));
                        else
                            return (byte)(oldByte);
                    }
                },

                new ByteOperation()
                {
                    Name = "Сумма",
                    SelectedOperation = (oldByte, newByte, opacity, flag) =>
                    {
                        if (flag)
                            return (byte)(oldByte + newByte * opacity);
                        else
                            return (byte)(oldByte);
                    }
                },
                new ByteOperation()
                {
                    Name = "Вычитание",
                    SelectedOperation = (oldByte, newByte, opacity, flag) =>
                    {
                        if (flag)
                            return (byte)(oldByte - newByte * opacity);
                        else
                            return (byte)(oldByte);
                    }
                },
                new ByteOperation()
                {
                    Name = "Максимум",
                    SelectedOperation = (oldByte, newByte, opacity, flag) =>
                    {
                        if (flag)
                            return byte.Max(oldByte, (byte)(newByte * opacity));
                        else
                            return (byte)(oldByte);
                    }
                },
                new ByteOperation()
                {
                    Name = "Геометрическое",
                    SelectedOperation = (oldByte, newByte, opacity, flag) =>
                    {
                        if (flag)
                            return (byte)Math.Sqrt(oldByte * newByte * opacity);
                        else
                            return (byte)(oldByte);
                    }
                },
                new ByteOperation()
                {
                    Name = "Арифметическое",
                    SelectedOperation = (oldByte, newByte, opacity, flag) =>
                    {
                        if (flag)
                            return (byte)((oldByte + newByte * opacity)/2);
                        else
                            return (byte)(oldByte);
                    }
                },

            };
        }
    }

}
