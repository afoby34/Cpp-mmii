#include <iostream>
#include <string>
#include <algorithm>
#include <cmath>
#include <limits>

#ifdef _WIN32
#include <windows.h>
#endif

using namespace std;

class PRICE {
private:
    string productName;
    string shopName;
    double price;
public:
    PRICE() : price(0.0) {}

    // Методы доступа
    string getProductName() const { return productName; }
    string getShopName() const { return shopName; }
    double getPrice() const { return price; }

    void setProductName(const string& n) { productName = n; }
    void setShopName(const string& n) { shopName = n; }
    void setPrice(double p) { price = p; }

    // Перегруженные операторы ввода/вывода
    friend ostream& operator<<(ostream& os, const PRICE& p);
    friend istream& operator>>(istream& is, PRICE& p);
};

ostream& operator<<(ostream& os, const PRICE& p) {
    os << "Товар: " << p.productName 
       << " | Магазин: " << p.shopName 
       << " | Цена: " << p.price << " руб.";
    return os;
}

istream& operator>>(istream& is, PRICE& p) {
    cout << "  Название товара: ";
    getline(is >> ws, p.productName);
    cout << "  Название магазина: ";
    getline(is >> ws, p.shopName);
    cout << "  Цена (руб): ";
    is >> p.price;
    return is;
}

// Вспомогательная сортировка по названию магазина
void sort_by_shop(PRICE items[], int count) {
    sort(items, items + count, [](const PRICE& a, const PRICE& b) {
        return a.getShopName() < b.getShopName();
    });
}

int main() {
#ifdef _WIN32
    SetConsoleCP(65001);
    SetConsoleOutputCP(65001);
#endif

    const int MAX = 8;
    PRICE items[MAX];
    int count = 0;
    int choice;

    do {
        cout << "\n=== ГЛАВНОЕ МЕНЮ ===\n";
        cout << "1. Добавить 1 товар (макс " << MAX << ")\n";
        cout << "2. Вывести все товары (по алфавиту магазинов)\n";
        cout << "3. Поиск по названию магазина\n";
        cout << "4. Поиск по названию товара\n";
        cout << "5. Поиск по цене\n";
        cout << "0. Выход\n";
        cout << "Ваш выбор: ";
        cin >> choice;
        cin.ignore(numeric_limits<streamsize>::max(), '\n'); // чистим буфер

        switch (choice) {
        case 1: {
            if (count >= MAX) {
                cout << "⚠️ Лимит: " << MAX << " товаров уже в базе.\n";
                break;
            }
            cout << "\n--- Добавление товара #" << count + 1 << " ---\n";
            cin >> items[count];
            count++;
            sort_by_shop(items, count); // авто-сортировка по заданию
            cout << "✅ Товар добавлен. В базе: " << count << "/" << MAX << "\n";
            break;
        }
        case 2: {
            if (count == 0) { cout << "⚠️ База пуста. Добавьте товары (пункт 1).\n"; break; }
            cout << "\n--- Все товары (отсортированы по магазину) ---\n";
            for (int i = 0; i < count; ++i) cout << items[i] << endl;
            break;
        }
        case 3: {
            if (count == 0) { cout << "⚠️ База пуста.\n"; break; }
            string target;
            cout << "\nВведите название магазина: ";
            getline(cin, target);
            bool found = false;
            cout << "\n--- Результаты ---\n";
            for (int i = 0; i < count; ++i) {
                if (items[i].getShopName() == target) {
                    cout << items[i] << endl;
                    found = true;
                }
            }
            if (!found) cout << "❌ Магазин \"" << target << "\" не найден.\n";
            break;
        }
        case 4: {
            if (count == 0) { cout << "⚠️ База пуста.\n"; break; }
            string target;
            cout << "\nВведите название товара: ";
            getline(cin, target);
            bool found = false;
            cout << "\n--- Результаты ---\n";
            for (int i = 0; i < count; ++i) {
                if (items[i].getProductName() == target) {
                    cout << items[i] << endl;
                    found = true;
                }
            }
            if (!found) cout << "❌ Товар \"" << target << "\" не найден.\n";
            break;
        }
        case 5: {
            if (count == 0) { cout << "⚠️ База пуста.\n"; break; }
            double target;
            cout << "\nВведите цену для поиска: ";
            cin >> target;
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            bool found = false;
            cout << "\n--- Результаты ---\n";
            for (int i = 0; i < count; ++i) {
                // Допуск 0.01 для безопасного сравнения double
                if (abs(items[i].getPrice() - target) < 0.01) {
                    cout << items[i] << endl;
                    found = true;
                }
            }
            if (!found) cout << "❌ Товаров с ценой " << target << " руб. не найдено.\n";
            break;
        }
        case 0:
            cout << "👋 Завершение работы...\n";
            break;
        default:
            cout << "⚠️ Неверный пункт. Попробуйте снова.\n";
        }
    } while (choice != 0);

    return 0;
}