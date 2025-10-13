# OnlineShop

OnlineShop jest projektem REST API sklepu internetowego. Do implementacji serwisów wykorzystany został język **C# (.NET w wersji 8.0)**, oraz pakiety
**Entity Framework Core** i **JWT**. System umożliwia przeglądanie listy produktów znajdujących się w katalogu, rejestracje i logowanie użytkowników,
zarządzanie produktami, koszykiem, wishlistą oraz zamówieniami.

Obsługiwane role użytkowników to: **Customer**, **Admin** oraz  **SalesDepartmentWorker**. Dzięki takiemu rozwiązaniu możliwa kontrola dostępu
do poszczególnych funkcjonalności.

API jest udokumentowane przy pomocy **Swagger UI** i może być łatwo zintegrowane z aplikacją webową lub mobilną. Dane przechowywane są
w bazie **SQL Server**, A architektura systemu została oparta na ASP.NET Core Web Api. Projekt jest częścią realizacji zadania akademickiego
i stanowi przykład implementacji nowoczesnych aplikacji e-commerce w technologii .NET w architekturze mikroserwisów.

# Repozytorium zawiera:
-**Serwis Catalog** - Przechowuje listę przedmiotów oferowanych przez sklep.
-**Serwis Identity** - Serwis odpowiedzialny za konta użytkowników oraz wydawanie tokenów JWT.
-**Serwis Shopping** - Odpowiedzialny za zarządzanie zamówieniami, koszykami i wishlistami użytkowników.
-**Komplet testów jednostkowych** dla każdego z serwisów.

Obecna wersja projektu to: 1.3