# JuniorQuest

## Autorzy
 **Martyna Budrewicz** i **Paweł Nowak**

 ## Wizja
 - [Wizja](docs/vision.md)

## Struktura projektu

| Katalog | Przeznaczenie |
| --- | --- |
| `src/ui/dashboard/` | Główny ekran ze statusami aplikacji i proponowanymi działaniami. |
| `src/ui/database-view/` | Widok danych w osobnym oknie. |
| `src/features/applications/` | Śledzenie aplikacji i zmian ich statusów. |
| `src/features/job-listings/` | Zbieranie i filtrowanie ofert pracy. |
| `src/features/user-profile/` | Profil użytkownika wykorzystywany przy dopasowywaniu ofert i CV. |
| `src/features/cv-personalization/` | Dostosowywanie CV do wymagań wybranej oferty. |
| `src/features/agent/` | Koordynacja zadań automatycznych. |
| `src/data/database/` | Połączenie z bazą i dostęp do danych. Rozważany dostawca to Firebase. |
| `src/config/` | Konfiguracja połączenia. Klucz będzie dostarczany poza repozytorium. |

## Pulpit w terminalu

Uruchom z katalogu głównego projektu. Wymagany jest .NET 10 SDK.

```sh
dotnet run --project src/ui/dashboard
```

Dodaj `-- --demo`, aby zobaczyć przykładowe aktualizacje statusów aplikacji.

## Pobieranie ofert

Instrukcja uruchomienia i schemat danych są w [dokumentacji agenta](src/features/agent/offer-ingestion/README.md).
