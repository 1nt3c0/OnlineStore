namespace proYP04
{
    public partial class Должность
    {
        public override string ToString()
        {
            return Наименование;
        }
    }

    public partial class Подразделение
    {
        public override string ToString()
        {
            return Наименование;
        }
    }
    
    public partial class Сотрудник
    {
        public override string ToString()
        {
            return Фамилия + " " + Имя; 
        }
    }

    public partial class Категории_товаров
    {
        public override string ToString()
        {
            return Наименование;
        }
    }

    public partial class Товары
    {
        public string ImagePath
        {
            get
            {
                if (string.IsNullOrEmpty(Картинка))
                    return "Allproduct/NotFound.png"; // заглушка

                return $"Allproduct/{Картинка}";
            }
        }
    }
}
