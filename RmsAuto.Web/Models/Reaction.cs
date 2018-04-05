public enum Reaction
{
   /// <summary>
   /// Не размещать в случае ошибки
   /// </summary>
   NotPush = 0,
   /// <summary>
   /// Размещать все что получится разместить
   /// </summary>
   AnyPush = 1,
   /// <summary>
   /// Строки рассматриваются по отдельности
   /// </summary>
   CheckRow = 2
}
