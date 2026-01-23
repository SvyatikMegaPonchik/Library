using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class Program
{
    public static string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    public static string usersFilePathTrue = Path.Combine(baseDirectory, "users.json");
    public static string libraryFilePath = "library.json";
    public static User currentUser = null;
    public static List<User> users = new List<User>();
    public static List<Books> listbooks = new List<Books>();

    public static void Main(string[] args)
    {
        LoadLibrary();
        LoadUsers();

        Console.WriteLine("---Добро пожаловать в библиотеку---");

        while (currentUser == null)
        {
            Console.WriteLine("\n1. Вход");
            Console.WriteLine("2. Регистрация");
            Console.Write("Выберите действие: ");
            try
            {
                int choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        Login();
                        break;
                    case 2:
                        Register();
                        break;
                    default:
                        Console.WriteLine("Ошибка: неверный выбор");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: неверный выбор");
            }
        }

        RunLibrary();
    }

    public static void Login()
    {
        Console.Write("\nВведите логин: ");
        string login = Console.ReadLine();

        Console.Write("Введите пароль: ");
        string password = Console.ReadLine();

        string encryptedPassword = CaesarCipher.Encrypt(password, 3);

        User user = users.Find(u => u.Login == login && u.Password == encryptedPassword);

        if (user != null)
        {
            currentUser = user;
            Console.WriteLine($"Добро пожаловать, {currentUser.Login}. ");
        }
        else
        {
            Console.WriteLine("Неверный логин или пароль!");
        }
    }

    public static void Register()
    {
        Console.Write("\nВведите логин: ");
        string login = Console.ReadLine();

        if (users.Exists(u => u.Login == login))
        {
            Console.WriteLine("Этот логин уже занят!");
            return;
        }

        Console.Write("Введите пароль: ");
        string password = Console.ReadLine();

        string encryptedPassword = CaesarCipher.Encrypt(password, 3);

        User newUser = new User
        {
            Login = login,
            Password = encryptedPassword,
            mylistbooks = new List<Books>(),
            readBooks = new List<Books>()
        };

        users.Add(newUser);
        SaveUsers();
        Console.WriteLine("Регистрация успешна! Теперь войдите в систему.");
    }

    public static void LoadUsers()
    {
        if (File.Exists(usersFilePathTrue))
        {
            try
            {
                string json = File.ReadAllText(usersFilePathTrue);
                users = JsonSerializer.Deserialize<List<User>>(json);
            }
            catch
            {
                users = new List<User>();
            }
        }
        else
        {
            users = new List<User>();
        }
    }

    public static void SaveUsers()
    {
        try
        {
            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
            File.WriteAllText(usersFilePathTrue, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении в файл:{ex.Message}");
            return;
        }
        
    }

    public static void LoadLibrary()
    {
        if (File.Exists(libraryFilePath))
        {
            try
            {
                string json = File.ReadAllText(libraryFilePath);
                listbooks = JsonSerializer.Deserialize<List<Books>>(json);
            }
            catch
            {
                // Если файл пуст или поврежден, инициализируем стандартными книгами
                listbooks = new List<Books>()
            {
                new Books("Крутая книга", "Геннадий Васильев", 2011, "Фантастика", 98, true, false),
                new Books("Грустная повесть", "Артем Петров", 1978, "Романтика", 456, true, false),
                new Books("Шерлок Холмс", "Грек Брамс", 1989, "Детектив", 678, true, false),
                new Books("Мастер и Маргарита", "Михаил Булгаков", 1966, "Роман", 480, true, false),
                new Books("1984", "Джордж Оруэлл", 1949, "Антиутопия", 320, true, false),
                new Books("Война и мир", "Лев Толстой", 1869, "Роман-эпопея", 1225, true, false),
                new Books("Гарри Поттер и философский камень", "Джоан Роулинг", 1997, "Фэнтези", 320, true, false)
            };
            }
        }
        else
        {
            // Если файла нет, создаем стандартную библиотеку
            listbooks = new List<Books>()
        {
            new Books("Крутая книга", "Геннадий Васильев", 2011, "Фантастика", 98, true, false),
            new Books("Грустная повесть", "Артем Петров", 1978, "Романтика", 456, true, false),
            new Books("Шерлок Холмс", "Грек Брамс", 1989, "Детектив", 678, true, false),
            new Books("Мастер и Маргарита", "Михаил Булгаков", 1966, "Роман", 480, true, false),
            new Books("1984", "Джордж Оруэлл", 1949, "Антиутопия", 320, true, false),
            new Books("Война и мир", "Лев Толстой", 1869, "Роман-эпопея", 1225, true, false),
            new Books("Гарри Поттер и философский камень", "Джоан Роулинг", 1997, "Фэнтези", 320, true, false)
        };
            SaveLibrary();
        }
    }

    public static void SaveLibrary()
    {
        try
        {
            string json = JsonSerializer.Serialize(listbooks, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            File.WriteAllText(libraryFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении библиотеки: {ex.Message}");
        }
    }

    static public void RunLibrary()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n---Библиотека книг ({currentUser.Login})---");
            Console.ResetColor();
            Console.WriteLine("1. Список книг в библиотеке");
            Console.WriteLine("2. Список книг у меня");
            Console.WriteLine("3. Взять книгу");
            Console.WriteLine("4. Вернуть книгу");
            Console.WriteLine("5. Добавить книгу в библиотеку");
            Console.WriteLine("6. Мои прочитанные книги");
            Console.WriteLine("7. Редактировать книгу в библиотеке");
            Console.WriteLine("8. Выход из аккаунта");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Выберите действие: ");
            Console.ResetColor();
            int turn = Convert.ToInt32(Console.ReadLine());

            switch (turn)
            {
                case 1:
                    ShowLibraryBooks();
                    break;
                case 2:
                    ShowMyBooks();
                    break;
                case 3:
                    TakeBook();
                    break;
                case 4:
                    ReturnBook();
                    break;
                case 5:
                    AddBook();
                    break;
                case 6:
                    ShowReadBooks();
                    break;
                case 7:
                    EditBook();
                    break;
                case 8:
                    Console.WriteLine("Выход из аккаунта...");
                    currentUser = null;
                    SaveUsers();
                    return;
                default:
                    Console.WriteLine("Неверный выбор!");
                    break;
            }
        }
    }

    static void ShowLibraryBooks()
    {
        Console.WriteLine();
        if (listbooks.Count != 0)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("=== Книги в библиотеке ===");
            Console.ResetColor();
            foreach (Books book in listbooks)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Название:{book.Name}");
                Console.WriteLine($"Автор:{book.Author}");
                Console.WriteLine($"Год Выпуска:{book.Year}");
                Console.WriteLine($"Жанр:{book.Genre}");
                Console.WriteLine($"Кол-во страниц:{book.Page}");

                Console.ForegroundColor = book.IsAvailaible ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine($"Доступна ли:{book.IsAvailaible}");
                Console.ForegroundColor = ConsoleColor.White;

                Console.ForegroundColor = book.IsRead ? ConsoleColor.Magenta : ConsoleColor.Gray;
                Console.WriteLine($"Прочитана:{book.IsRead}\n");
                Console.ResetColor();
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Библиотека пуста.");
            Console.ResetColor();
        }
    }

    static void ShowMyBooks()
    {
        Console.WriteLine();
        if (currentUser.mylistbooks.Count != 0)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("=== Мои книги ===");
            Console.ResetColor();
            foreach (Books book in currentUser.mylistbooks)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Название:{book.Name}");
                Console.WriteLine($"Автор:{book.Author}");
                Console.WriteLine($"Год Выпуска:{book.Year}");
                Console.WriteLine($"Жанр:{book.Genre}");
                Console.WriteLine($"Кол-во страниц:{book.Page}");

                Console.ForegroundColor = book.IsAvailaible ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine($"Доступна ли:{book.IsAvailaible}");
                Console.ForegroundColor = ConsoleColor.White;

                Console.ForegroundColor = book.IsRead ? ConsoleColor.Magenta : ConsoleColor.Gray;
                Console.WriteLine($"Прочитана:{book.IsRead}\n");
                Console.ResetColor();
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("У вас нет книг.");
            Console.ResetColor();
        }
    }

    static void TakeBook()
    {
        Console.WriteLine();
        if (listbooks.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Библиотека пуста.");
            Console.ResetColor();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("=== Доступные книги ===");
        Console.ResetColor();

        foreach (Books book in listbooks)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Название:{book.Name}");
            Console.WriteLine($"Автор:{book.Author}");
            Console.WriteLine($"Год Выпуска:{book.Year}");
            Console.WriteLine($"Жанр:{book.Genre}");
            Console.WriteLine($"Кол-во страниц:{book.Page}");

            Console.ForegroundColor = book.IsAvailaible ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"Доступна ли:{book.IsAvailaible}");
            Console.ForegroundColor = ConsoleColor.White;

            Console.ForegroundColor = book.IsRead ? ConsoleColor.Magenta : ConsoleColor.Gray;
            Console.WriteLine($"Прочитана:{book.IsRead}\n");
            Console.ResetColor();
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Введите название книги которую хотите взять: ");
        Console.ResetColor();
        string bookToTake = Console.ReadLine();

        Books foundBook = listbooks.Find(book => book.Name == bookToTake);
        if (foundBook != null && foundBook.IsAvailaible)
        {
            listbooks.Remove(foundBook);
            currentUser.mylistbooks.Add(foundBook);
            foundBook.IsAvailaible = false;

            bool alreadyRead = currentUser.readBooks.Exists(book => book.Name == foundBook.Name);

            if (!alreadyRead)
            {
                foundBook.IsRead = true;
                currentUser.readBooks.Add(foundBook);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Вы взяли книгу: {foundBook.Name}");
                Console.WriteLine($"Книга '{foundBook.Name}' отмечена как прочитанная!");
                Console.ResetColor();
            }
            else
            {
                foundBook.IsRead = true;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Вы взяли книгу: {foundBook.Name}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Книга '{foundBook.Name}' уже была прочитана вами ранее!");
                Console.ResetColor();
            }
        }
        else if (foundBook != null && !foundBook.IsAvailaible)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Эта книга уже взята!");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Такой книги нет!");
            Console.ResetColor();
        }

        SaveLibrary();
        SaveUsers();
    }

    static void ReturnBook()
    {
        Console.WriteLine();
        if (currentUser.mylistbooks.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("У вас нет книг");
            Console.ResetColor();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("=== Ваши книги для возврата ===");
        Console.ResetColor();

        foreach (Books book in currentUser.mylistbooks)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Название:{book.Name}");
            Console.WriteLine($"Автор:{book.Author}");
            Console.WriteLine($"Год Выпуска:{book.Year}");
            Console.WriteLine($"Жанр:{book.Genre}");
            Console.WriteLine($"Кол-во страниц:{book.Page}");

            Console.ForegroundColor = book.IsAvailaible ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"Доступна ли:{book.IsAvailaible}");
            Console.ForegroundColor = ConsoleColor.White;

            Console.ForegroundColor = book.IsRead ? ConsoleColor.Magenta : ConsoleColor.Gray;
            Console.WriteLine($"Прочитана:{book.IsRead}");
            Console.ResetColor();
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Введите название книги которую хотите вернуть: ");
        Console.ResetColor();
        string bookToReturn = Console.ReadLine();

        Books foundMyBook = currentUser.mylistbooks.Find(book => book.Name == bookToReturn);
        if (foundMyBook != null)
        {
            currentUser.mylistbooks.Remove(foundMyBook);
            listbooks.Add(foundMyBook);
            foundMyBook.IsAvailaible = true;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Вы вернули книгу: {foundMyBook.Name}");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("У вас нет такой книги");
            Console.ResetColor();
        }

        SaveLibrary();
        SaveUsers();
    }

    static void AddBook()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n---Добавление новой книги---");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Введите название книги: ");
        Console.ResetColor();
        string name = Console.ReadLine();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Введите автора книги: ");
        Console.ResetColor();
        string author = Console.ReadLine();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Введите год выпуска книги: ");
        Console.ResetColor();
        int year = Convert.ToInt32(Console.ReadLine());

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Введите жанр книги: ");
        Console.ResetColor();
        string genre = Console.ReadLine();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Введите количество страниц: ");
        Console.ResetColor();
        int pages = Convert.ToInt32(Console.ReadLine());

        Books newBook = new Books(name, author, year, genre, pages, true, false);

        Books existingBook = listbooks.Find(book => book.Name == name);

        if (existingBook == null)
        {
            listbooks.Add(newBook);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Книга '{name}' успешно добавлена в библиотеку!");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Такая книга уже есть в библиотеке!");
            Console.ResetColor();
        }

        SaveLibrary();
    }

    static void ShowReadBooks()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n---Мои прочитанные книги---");
        Console.ResetColor();
        if (currentUser.readBooks.Count > 0)
        {
            int totalPages = 0;

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Список прочитанных книг:");
            Console.ResetColor();
            foreach (Books book in currentUser.readBooks)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"• {book.Name} - {book.Author} ({book.Page} стр.)");
                Console.ResetColor();
                totalPages += book.Page;
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"\nИтого:");
            Console.WriteLine($"Прочитано книг: {currentUser.readBooks.Count}");
            Console.WriteLine($"Общее количество прочитанных страниц: {totalPages}");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("У вас нет прочитанных книг.");
            Console.ResetColor();
        }
    }

    static void EditBook()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n---Редактирование книги в библиотеке---");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Книги доступные для редактирования (в библиотеке):");
        Console.ResetColor();
        bool hasEditableBooks = false;

        for (int i = 0; i < listbooks.Count; i++)
        {
            if (listbooks[i].IsAvailaible)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{i + 1}. {listbooks[i].Name} - {listbooks[i].Author}");
                Console.ResetColor();
                hasEditableBooks = true;
            }
        }

        if (!hasEditableBooks)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Нет книг доступных для редактирования (все книги взяты)");
            Console.ResetColor();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("\nВведите номер книги для редактирования: ");
        Console.ResetColor();
        int editIndex = Convert.ToInt32(Console.ReadLine()) - 1;

        if (editIndex >= 0 && editIndex < listbooks.Count && listbooks[editIndex].IsAvailaible)
        {
            Books bookToEdit = listbooks[editIndex];

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\nТекущие данные книги:");
            Console.ResetColor();
            Console.WriteLine($"1. Название: {bookToEdit.Name}");
            Console.WriteLine($"2. Автор: {bookToEdit.Author}");
            Console.WriteLine($"3. Год выпуска: {bookToEdit.Year}");
            Console.WriteLine($"4. Жанр: {bookToEdit.Genre}");
            Console.WriteLine($"5. Количество страниц: {bookToEdit.Page}");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nЧто вы хотите изменить?");
            Console.ResetColor();
            Console.WriteLine("1. Название");
            Console.WriteLine("2. Автор");
            Console.WriteLine("3. Год выпуска");
            Console.WriteLine("4. Жанр");
            Console.WriteLine("5. Количество страниц");
            Console.WriteLine("6. Всё");

            Console.ForegroundColor = ConsoleColor.Yellow;
            int editChoice = Convert.ToInt32(Console.ReadLine());
            Console.ResetColor();

            switch (editChoice)
            {
                case 1:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Введите новое название: ");
                    Console.ResetColor();
                    bookToEdit.Name = Console.ReadLine();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Название изменено!");
                    Console.ResetColor();
                    break;
                case 2:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Введите нового автора: ");
                    Console.ResetColor();
                    bookToEdit.Author = Console.ReadLine();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Автор изменен!");
                    Console.ResetColor();
                    break;
                case 3:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Введите новый год выпуска: ");
                    Console.ResetColor();
                    bookToEdit.Year = Convert.ToInt32(Console.ReadLine());

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Год выпуска изменен!");
                    Console.ResetColor();
                    break;
                case 4:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Введите новый жанр: ");
                    Console.ResetColor();
                    bookToEdit.Genre = Console.ReadLine();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Жанр изменен!");
                    Console.ResetColor();
                    break;
                case 5:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Введите новое количество страниц: ");
                    Console.ResetColor();
                    bookToEdit.Page = Convert.ToInt32(Console.ReadLine());

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Количество страниц изменено!");
                    Console.ResetColor();
                    break;
                case 6:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Введите новое название: ");
                    Console.ResetColor();
                    bookToEdit.Name = Console.ReadLine();

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Введите нового автора: ");
                    Console.ResetColor();
                    bookToEdit.Author = Console.ReadLine();

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Введите новый год выпуска: ");
                    Console.ResetColor();
                    bookToEdit.Year = Convert.ToInt32(Console.ReadLine());

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Введите новый жанр: ");
                    Console.ResetColor();
                    bookToEdit.Genre = Console.ReadLine();

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Введите новое количество страниц: ");
                    Console.ResetColor();
                    bookToEdit.Page = Convert.ToInt32(Console.ReadLine());

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Все данные книги изменены!");
                    Console.ResetColor();
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Неверный выбор!");
                    Console.ResetColor();
                    break;
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Неверный номер книги или книга взята другим пользователем!");
            Console.ResetColor();
        }

        SaveLibrary();
    }
}

public class Books
{
    private string _name;
    public string Name
    {
        get { return _name; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Название не может быть пустым!");
                Console.ResetColor();
            }
            else _name = value;
        }
    }

    private string _author;
    public string Author
    {
        get { return _author; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Автор не может быть пустым!");
                Console.ResetColor();
            }
            else _author = value;
        }
    }

    private int _year;
    public int Year
    {
        get { return _year; }
        set
        {
            if (value < 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Год выпуска не может быть отрицательным!");
                Console.ResetColor();
            }
            else _year = value;
        }
    }

    private string _genre;
    public string Genre
    {
        get { return _genre; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Жанр не может быть пустым!");
                Console.ResetColor();
            }
            else _genre = value;
        }
    }

    private int _page;
    public int Page
    {
        get { return _page; }
        set
        {
            if (value <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Кол-во страниц не может быть отрицательным или нулем!");
                Console.ResetColor();
            }
            else _page = value;
        }
    }

    private bool _isAvailaible;
    public bool IsAvailaible
    {
        get { return _isAvailaible; }
        set
        {
            if (value == true || value == false)
                _isAvailaible = value;
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Значение может быть только true или false!");
                Console.ResetColor();
            }
        }
    }

    private bool _isRead;
    public bool IsRead
    {
        get { return _isRead; }
        set
        {
            if (value == true || value == false)
                _isRead = value;
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Значение может быть только true или false!");
                Console.ResetColor();
            }
        }
    }

    public void ChangeAvailaibleBook(bool newBool)
    {
        IsAvailaible = newBool;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Значение доступности стал:{IsAvailaible}");
        Console.ResetColor();
    }

    public Books(string name, string author, int year, string genre, int page, bool isAvailaible, bool isRead)
    {
        Name = name;
        Author = author;
        Year = year;
        Genre = genre;
        Page = page;
        IsAvailaible = isAvailaible;
        IsRead = isRead;
    }

   public Books()
    {

    }
}

public class User
{
    public string Login { get; set; }
    public string Password { get; set; }
    public List<Books> mylistbooks { get; set; }
    public List<Books> readBooks { get; set; }
}

public static class CaesarCipher
{
    public static string Encrypt(string text, int shift)
    {
        char[] buffer = text.ToCharArray();

        for (int i = 0; i < buffer.Length; i++)
        {

            buffer[i] = (char)(buffer[i] + shift);
        }

        return new string(buffer);
    }

    public static string Decrypt(string text, int shift)
    {
        return Encrypt(text, -shift);
    }
}

