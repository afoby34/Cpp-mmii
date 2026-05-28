#include <iostream>
#include <string>
#include <stdexcept>

using namespace std;


template <typename T>
class MyVector {
private:
    T* data;
    size_t cap;

public:
    explicit MyVector(size_t initial_cap = 4) : data(new T[initial_cap]), cap(initial_cap) {}
    ~MyVector() { delete[] data; }

    MyVector(const MyVector& other) : data(new T[other.cap]), cap(other.cap) {
        for (size_t i = 0; i < cap; ++i) data[i] = other.data[i];
    }

    MyVector& operator=(const MyVector& other) {
        if (this != &other) {
            delete[] data;
            cap = other.cap;
            data = new T[cap];
            for (size_t i = 0; i < cap; ++i) data[i] = other.data[i];
        }
        return *this;
    }

    T& operator[](size_t idx) { return data[idx]; }
    const T& operator[](size_t idx) const { return data[idx]; }
    size_t capacity() const { return cap; }

    void resize(size_t new_cap) {
        if (new_cap == 0) throw invalid_argument("Capacity must be > 0");
        T* new_data = new T[new_cap];
        for (size_t i = 0; i < cap; ++i) new_data[i] = data[i];
        delete[] data;
        data = new_data;
        cap = new_cap;
    }
};


template <typename T>
class MyDeque {
private:
    MyVector<T> buf;
    size_t head, tail, count;

    size_t next(size_t idx) const { return (idx + 1) % buf.capacity(); }
    size_t prev(size_t idx) const { return (idx == 0) ? buf.capacity() - 1 : idx - 1; }

    void ensureCapacity() {
        if (count == buf.capacity()) {
            size_t new_cap = buf.capacity() * 2;
            MyVector<T> new_buf(new_cap);
            for (size_t i = 0; i < count; ++i)
                new_buf[i] = buf[(head + i) % buf.capacity()];
            buf = new_buf;
            head = 0;
            tail = count;
        }
    }

public:
    explicit MyDeque(size_t initial_cap = 4) : buf(initial_cap), head(0), tail(0), count(0) {}

    bool empty() const { return count == 0; }
    size_t size() const { return count; }

    void push_front(const T& val) {
        ensureCapacity();
        head = prev(head);
        buf[head] = val;
        ++count;
    }

    void push_back(const T& val) {
        ensureCapacity();
        buf[tail] = val;
        tail = next(tail);
        ++count;
    }

    void pop_front() {
        if (empty()) throw out_of_range("Pop front from empty deque");
        head = next(head);
        --count;
    }

    void pop_back() {
        if (empty()) throw out_of_range("Pop back from empty deque");
        tail = prev(tail);
        --count;
    }

    T& front() {
        if (empty()) throw out_of_range("Access front of empty deque");
        return buf[head];
    }

    T& back() {
        if (empty()) throw out_of_range("Access back of empty deque");
        return buf[prev(tail)];
    }

    void show() const {
        cout << "[ ";
        for (size_t i = 0; i < count; ++i)
            cout << buf[(head + i) % buf.capacity()] << " ";
        cout << "]\n";
    }
};

// Шаблонное меню для управления деком любого типа
template <typename T>
void runMenu() {
    MyDeque<T> dq;
    int choice;
    do {
        cout << "\n=== УПРАВЛЕНИЕ ДЕКОМ ===\n";
        cout << "1. Добавить в конец (push_back)\n";
        cout << "2. Добавить в начало (push_front)\n";
        cout << "3. Удалить с конца (pop_back)\n";
        cout << "4. Удалить с начала (pop_front)\n";
        cout << "5. Показать первый элемент (front)\n";
        cout << "6. Показать последний элемент (back)\n";
        cout << "7. Показать всё содержимое\n";
        cout << "8. Размер и проверка на пустоту\n";
        cout << "9. Тест исключений (попытка операции над пустым деком)\n";
        cout << "10. Выход\n";
        cout << "Выбор: ";
        
        if (!(cin >> choice)) {
            cin.clear();
            cin.ignore(100, '\n');
            cout << "Некорректный ввод.\n";
            continue;
        }

        try {
            T val;
            switch (choice) {
                case 1:
                    cout << "Введите значение: "; cin >> val;
                    dq.push_back(val);
                    cout << "Добавлено.\n";
                    break;
                case 2:
                    cout << "Введите значение: "; cin >> val;
                    dq.push_front(val);
                    cout << "Добавлено.\n";
                    break;
                case 3:
                    dq.pop_back();
                    cout << "Удалён последний элемент.\n";
                    break;
                case 4:
                    dq.pop_front();
                    cout << "Удалён первый элемент.\n";
                    break;
                case 5:
                    cout << "Front: " << dq.front() << "\n";
                    break;
                case 6:
                    cout << "Back: " << dq.back() << "\n";
                    break;
                case 7:
                    cout << "Содержимое: ";
                    dq.show();
                    break;
                case 8:
                    cout << "Размер: " << dq.size() << ", Пусто: " << (dq.empty() ? "да" : "нет") << "\n";
                    break;
                case 9: {
                    cout << "Генерация исключения (pop_front из пустого):\n";
                    MyDeque<T> empty_test;
                    empty_test.pop_front();
                    break;
                }
                case 10:
                    cout << "Выход.\n";
                    break;
                default:
                    cout << "Неверный пункт меню.\n";
            }
        } catch (const exception& e) {
            cout << "[ИСКЛЮЧЕНИЕ] " << e.what() << "\n";
        }
    } while (choice != 10);
}

int main() {
    int typeChoice;
    cout << "Выберите тип данных для дека:\n1. int\n2. double\n3. std::string\nВыбор: ";
    cin >> typeChoice;

    // Инстанцирование шаблона для выбранного типа
    switch (typeChoice) {
        case 1: runMenu<int>(); break;
        case 2: runMenu<double>(); break;
        case 3: runMenu<string>(); break;
        default:
            cout << "Неверный выбор. Запускаю для string.\n";
            runMenu<string>();
    }
    return 0;
}