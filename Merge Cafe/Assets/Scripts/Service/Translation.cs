using Enums;
using System.Collections.Generic;

namespace Service
{
    public static class Translation
    {
        private static Dictionary<ItemType, Dictionary<int, ItemDescription>> _itemDescriptions = new Dictionary<ItemType, Dictionary<int, ItemDescription>>
        {
            [ItemType.BakeryProduct] = new Dictionary<int, ItemDescription>
            {
                [1] = new ItemDescription("Сахар", "Соль жизни в том, что она не сахар."),
                [2] = new ItemDescription("Тесто", "Тили-тили-тесто!"),
                [3] = new ItemDescription("Крендель", "Колобок, упавший с дуба."),
                [4] = new ItemDescription("Печенье", "Сделано из масла и любви."),
                [5] = new ItemDescription("Печенье с крошкой", "С восхитительными кусочками шоколада."),
                [6] = new ItemDescription("Слойка", "С сюрпризом внутри."),
                [7] = new ItemDescription("Вафля", "У бабушки был крайне неудачный день: и вафли не получились, и ноутбук зря испортила."),
                [8] = new ItemDescription("Маффин", "День удался."),
                [9] = new ItemDescription("Халва", "Сколько ни говори «халва» — во рту сладко не станет."),
                [10] = new ItemDescription("Пирог", "Жизнь — это пирог, в котором преобладает слой нелепости."),
            },
            [ItemType.BakeryProductWithCream] = new Dictionary<int, ItemDescription>
            {
                [1] = new ItemDescription("Яйцо", "Не простое, а золотое."),
                [2] = new ItemDescription("Крем", "Как омолаживающий, только ещё и съедобный."),
                [3] = new ItemDescription("Зефир", "Ты, смотри, не унывай — ешь зефир и отдыхай!"),
                [4] = new ItemDescription("Мусс", "Для себя любимого. Или любимой :)"),
                [5] = new ItemDescription("Пряник", "Не имей сто печенек, а имей сто пряников!"),
                [6] = new ItemDescription("Синнабон", "Включён в список 50-ти удовольствий жизни."),
                [7] = new ItemDescription("Пончик", "Счастье — это знать, что есть пончики."),
                [8] = new ItemDescription("Макарун", "Как только его не называют — макарон, макарун, макаронс, макарони…"),
                [9] = new ItemDescription("Рулет", "Сладкая колбаска."),
                [10] = new ItemDescription("Торт", "То, из-за чего мы просыпаемся ночью голодными."),
            },
            [ItemType.Box] = new Dictionary<int, ItemDescription>
            {
                [1] = new ItemDescription("Бумага", "Контракт на доставку эм... товара. Какого? Мы и сами не знаем."),
                [2] = new ItemDescription("Коробка", "А может мне повезёт?"),
                [3] = new ItemDescription("Подарочная коробка", "Для тебя."),
            },
            [ItemType.Brilliant] = new Dictionary<int, ItemDescription>
            {
                [1] = new ItemDescription("Бриллиант", "Что это там блестит?"),
                [2] = new ItemDescription("Парочка бриллиантов", "А может больше?"),
                [3] = new ItemDescription("Горсть бриллиантов", "Это ещё не предел!"),
                [4] = new ItemDescription("Куча бриллиантов", "Хватит на яхту."),
                [5] = new ItemDescription("Гора бриллиантов", "Да вы богаты!"),
            },
            [ItemType.Coffee] = new Dictionary<int, ItemDescription>
            {
                [1] = new ItemDescription("Зерно кофе", "И зачем оно пригодилось клиентам?"),
                [2] = new ItemDescription("Растворимый кофе", "Отборный."),
                [3] = new ItemDescription("Стакан кофе", "В одноразовом стаканчике. Неэкологично."),
                [4] = new ItemDescription("Чашка кофе", "Самый лёгкий способ поднять с утра человека. Вылил и беги!"),
                [5] = new ItemDescription("Кофе с печеньем", "Что может быть вкуснее?"),
                [6] = new ItemDescription("Кофейный коктейль", "Подаётся в автомате. Уже проиграл в него зарплату."),
                [7] = new ItemDescription("Капучино", "Кофейная классика."),
                [8] = new ItemDescription("Капучино v2.0", "Восьмидесятого уровня!"),
                [9] = new ItemDescription("Кофейник", "Теперь в 3 раза больше, для насущной бодрости."),
                [10] = new ItemDescription("Кофейник", "Лучшее кофе в заведении."),
            },
            [ItemType.FastFood] = new Dictionary<int, ItemDescription>
            {
                [1] = new ItemDescription("Хлеб", "Такая булка, большая и румяная."),
                [2] = new ItemDescription("Ломоть хлеба", "Собственно говоря, а в чём выгода?"),
                [3] = new ItemDescription("Бутерброд", "Опять маслом вниз..."),
                [4] = new ItemDescription("Сэндвич", "Почти как бургер."),
                [5] = new ItemDescription("Хот-дог", "Мне, пожалуйста, два хот-дога, один с горчицей, а другой без."),
                [6] = new ItemDescription("Тако", "Немного Мексики не помешает."),
                [7] = new ItemDescription("Шаверма", "Любимое блюдо студентов."),
                [8] = new ItemDescription("Большой бутерброд", "Попрощайтесь с фигурой..."),
                [9] = new ItemDescription("Картошка ФРИ", "Тает во рту. А ещё с майонезиком, ммм..."),
                [10] = new ItemDescription("Пицца", "Дон-Пицерроне"),
            },
            [ItemType.Key] = new Dictionary<int, ItemDescription>
            {
                [1] = new ItemDescription("Ключ", "Обычный уродливый ключ для таких же уродливых замков."),
                [2] = new ItemDescription("Золотой ключ", "Прямиком из Буратино."),
                [3] = new ItemDescription("Серебряный ключ", "Впервые не забыли взять по уходу из дома, осталось только голову не забыть."),
                [4] = new ItemDescription("Техно-ключ", "А такой вообще существует?"),
                [5] = new ItemDescription("Ключ от сейфа", "Но также подойдёт и для наших супер-прочных сейф-замков."),
            },
            [ItemType.Lock] = new Dictionary<int, ItemDescription>
            {
                [1] = new ItemDescription("Хлипкий замок", "Именем кулинарного бога я приказываю тебе — исчезни!"),
                [2] = new ItemDescription("Золотой замок", "Да, мешает, зато посмотри, как блестит!"),
                [3] = new ItemDescription("Серебряный замок", "Преграда посерьёзнее..."),
                [4] = new ItemDescription("Техно-замок", "Без особого инструмента его не открыть. До чего дошли технологии!"),
                [5] = new ItemDescription("Сейф-замок", "Очень прочный. Его отделили от сейфа?"),
            },
            [ItemType.OpenPresent] = new Dictionary<int, ItemDescription>
            {
                [1] = new ItemDescription("Синий подарок", "Ну чего ты ждёшь? Скорее смотри, что там внутри!"),
                [2] = new ItemDescription("Красный подарок", "Ну что там? Нам тоже интересно!"),
                [3] = new ItemDescription("Золотой подарок", "Прямо сейчас ты станешь самым богатым кулинаром на планете."),
            },
            [ItemType.Oven] = new Dictionary<int, ItemDescription>
            {
                [1] = new ItemDescription("Духовая печь", "А ты хранишь в своей духовке сковородки?"),
                [2] = new ItemDescription("Духовая печь", "А ты хранишь в своей духовке сковородки?"),
                [3] = new ItemDescription("Духовая печь", "А ты хранишь в своей духовке сковородки?"),
                [4] = new ItemDescription("Духовая печь", "А ты хранишь в своей духовке сковородки?"),
                [5] = new ItemDescription("Духовая печь", "А ты хранишь в своей духовке сковородки?"),
                [6] = new ItemDescription("Духовая печь", "А ты хранишь в своей духовке сковородки?"),
            },
            [ItemType.Present] = new Dictionary<int, ItemDescription>
            {
                [1] = new ItemDescription("Синий подарок", "Надо открыть! А может потерпеть и сначала объединить? Нет, открываю! Хотя..."),
                [2] = new ItemDescription("Красный подарок", "Вот бы мне такой на день рождения."),
                [3] = new ItemDescription("Золотой подарок", "Лучшему объединяльщику месяца."),
            },
            [ItemType.Star] = new Dictionary<int, ItemDescription>
            {
                [1] = new ItemDescription("Звезда", "Твой путь к успеху."),
                [2] = new ItemDescription("Пара звёзд", "Нужно больше звёзд. Ещё БОЛЬШЕ!"),
                [3] = new ItemDescription("Горстка звёзд", "Почему мы не можем просто вырезать новые звёзды?"),
                [4] = new ItemDescription("Куча звёзд", "Они с потолка падают? Алло, где логика?"),
                [5] = new ItemDescription("Гора звёзд", "Ну теперь-то наедимся вдоволь."),
            },
            [ItemType.Tea] = new Dictionary<int, ItemDescription>
            {
                [1] = new ItemDescription("Чайный лист", "Такой крохотный и беззащитный."),
                [2] = new ItemDescription("Чайный пакетик", "Погрузите его в кипяток."),
                [3] = new ItemDescription("Чёрный чай", "Прямо как моя душа."),
                [4] = new ItemDescription("Зелёный чай", "Согревает."),
                [5] = new ItemDescription("Чай с лимоном", "— Мне, пожалуйста, чивирзднеркильмерк с лимоном!\n— Чивирзднеркильмерк с чем?"),
                [6] = new ItemDescription("Заварник", "Захвати для своих друзей."),
                [7] = new ItemDescription("Холодный чай", "Вы не поверите, но это не просто чай, который остыл!"),
                [8] = new ItemDescription("Цветочный чай", "Успокаивает и затуманивает разум..."),
                [9] = new ItemDescription("Чай с бадьяном", "Чудесный витаминный напиток."),
                [10] = new ItemDescription("Элитный чай", "Открываем страшную тайну: это самый обычный чай в элитной обёртке!"),
            },
            [ItemType.Teapot] = new Dictionary<int, ItemDescription>
            {
                [1] = new ItemDescription("Чайник", "А Вы тоже кричите чайнику со свистком: «Та иду я, иду!»?"),
                [2] = new ItemDescription("Чайник", "А Вы тоже кричите чайнику со свистком: «Та иду я, иду!»?"),
                [3] = new ItemDescription("Чайник", "А Вы тоже кричите чайнику со свистком: «Та иду я, иду!»?"),
                [4] = new ItemDescription("Чайник", "А Вы тоже кричите чайнику со свистком: «Та иду я, иду!»?"),
                [5] = new ItemDescription("Чайник", "А Вы тоже кричите чайнику со свистком: «Та иду я, иду!»?"),
                [6] = new ItemDescription("Чайник", "А Вы тоже кричите чайнику со свистком: «Та иду я, иду!»?"),
            },
            [ItemType.Toaster] = new Dictionary<int, ItemDescription>
            {
                [1] = new ItemDescription("Тостер", "Всего лишь луч смерти с недостаточным энергопитанием."),
                [2] = new ItemDescription("Тостер", "Всего лишь луч смерти с недостаточным энергопитанием."),
                [3] = new ItemDescription("Тостер", "Всего лишь луч смерти с недостаточным энергопитанием."),
                [4] = new ItemDescription("Тостер", "Всего лишь луч смерти с недостаточным энергопитанием."),
                [5] = new ItemDescription("Тостер", "Всего лишь луч смерти с недостаточным энергопитанием."),
                [6] = new ItemDescription("Тостер", "Всего лишь луч смерти с недостаточным энергопитанием."),
            },
            [ItemType.TrashCan] = new Dictionary<int, ItemDescription>
            {
                [1] = new ItemDescription("Мусорный бак", "Готовить просто! Выкладываем продукт на сковороду, выбрасываем упаковку. Думаем... Лезем обратно в мусорку за упаковкой, чтобы прочитать инструкцию."),
                [2] = new ItemDescription("Мусорный бак", "Готовить просто! Выкладываем продукт на сковороду, выбрасываем упаковку. Думаем... Лезем обратно в мусорку за упаковкой, чтобы прочитать инструкцию."),
                [3] = new ItemDescription("Мусорный бак", "Готовить просто! Выкладываем продукт на сковороду, выбрасываем упаковку. Думаем... Лезем обратно в мусорку за упаковкой, чтобы прочитать инструкцию."),
                [4] = new ItemDescription("Мусорный бак", "Готовить просто! Выкладываем продукт на сковороду, выбрасываем упаковку. Думаем... Лезем обратно в мусорку за упаковкой, чтобы прочитать инструкцию."),
                [5] = new ItemDescription("Мусорный бак", "Готовить просто! Выкладываем продукт на сковороду, выбрасываем упаковку. Думаем... Лезем обратно в мусорку за упаковкой, чтобы прочитать инструкцию."),
                [6] = new ItemDescription("Мусорный бак", "Готовить просто! Выкладываем продукт на сковороду, выбрасываем упаковку. Думаем... Лезем обратно в мусорку за упаковкой, чтобы прочитать инструкцию."),
            },
        };

        private static Dictionary<ItemType, string> _itemNames = new Dictionary<ItemType, string>
        {
            [ItemType.Star] = "звёзды",
            [ItemType.Brilliant] = "бриллианты",
        };

        public static string GetItemTitle(ItemType type) => _itemNames[type];

        public static ItemDescription GetItemDescription(ItemType type, int level)
        {
            if (_itemDescriptions.ContainsKey(type) && _itemDescriptions[type].ContainsKey(level))
                return _itemDescriptions[type][level];
            return null;
        }

        public class ItemDescription
        {
            public string Title { get; }
            public string Description { get; }

            public ItemDescription(string title, string description)
            {
                Title = title;
                Description = description;
            }
        }
    }
}