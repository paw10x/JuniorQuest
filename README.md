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
| `src/data/database/` | Adapter C# do centralnej bazy Firebase SQL Connect. |
| `src/config/` | Konfiguracja połączenia. Klucz będzie dostarczany poza repozytorium. |

## Uruchamianie pulpitu

W Riderze kliknij prawym przyciskiem projekt **Dashboard** w oknie Solution
i wybierz **Run 'Dashboard'**.

Możesz też uruchomić go z terminala, z katalogu głównego repozytorium:

```sh
dotnet run --project src/ui/dashboard
```

Aby zobaczyć przykładowe aktualizacje statusów aplikacji:

```sh
dotnet run --project src/ui/dashboard -- --demo
```

`OfferIngestion` jest biblioteką i nie można go jeszcze uruchomić samodzielnie.

## Pobieranie ofert

Konfiguracja Firebase SQL Connect, emulator i test połączenia w Riderze:
[docs/firebase.md](docs/firebase.md).

Instrukcja uruchomienia i schemat danych są w [dokumentacji agenta](src/features/agent/offer-ingestion/README.md).
