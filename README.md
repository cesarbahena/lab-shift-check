# ShiftCheck

ShiftCheck is the Android handover client for QuimiOS. A technician signs in, reviews exams that have not been validated, selects the work that needs follow-up, records a reason for each exam, and leaves notes for the next shift. The handover is stored in Hub with its shift, author, date, and selected exams.

## Run

The project targets `net9.0-android` and requires the .NET 9 SDK, the `maui-android` workload, an Android SDK, and a JDK. Run `dotnet build -f net9.0-android` from this directory after installing those dependencies.

On the login screen, enter the Hub API base URL ending in `/api/`, along with a Hub username and password. The address is remembered on the device. The app requires HTTPS in release builds; debug builds can use HTTP for local development. The Hub must have at least one active shift and user, and synchronized exams for the pending-work screen.

## Handover flow

1. Sign in and refresh the pending-exam list. A failed request shows an error instead of an empty-work message.
2. Select one or more exams. The selection is visible on each card and carried to the handover form.
3. Choose a shift, add notes, and edit each exam's reason. Save once and check the resulting handover in `GET /api/shifthandovers`.

The app uses `GET /api/exams/pending`, `GET /api/shifts`, `POST /api/shifthandovers`, and `POST /api/auth/login`. The Hub login response supplies the authenticated user ID used for the handover. A failed save keeps the form available for review; confirm the result in Hub before retrying if the connection dropped after submission.

## Current limits

The app has an Android build and a Hub API integration smoke test, but no device interaction test in this repository. It does not yet support acknowledging another technician's handover or offline drafts. See the Hub README for server setup and sample data.
