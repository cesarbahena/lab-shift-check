# ShiftCheck

ShiftCheck is the Android handover client for QuimiOS. A technician signs in, reviews exams that have not been validated, selects the work that needs follow-up, records a reason for each exam, and leaves notes for the next shift. The handover is stored in Hub with its shift, author, date, and selected exams.

## Run

Open `ShiftCheck.sln` in Visual Studio 2019 with the Mobile development with .NET workload and Android SDK installed. Build the Android project and run it on an emulator or device. The app uses Xamarin.Forms 4.0 and targets Android 9 (API 28), with Android 5.0 as its minimum.

On the login screen, enter the Hub API base URL ending in `/api/`, along with a Hub username and password. The address is remembered on the device. The app requires HTTPS in release builds; debug builds can use HTTP for local development. The Hub must have at least one active shift and user, and synchronized exams for the pending-work screen.

## Handover flow

1. Sign in and refresh the pending-exam list. A failed refresh keeps the last list and selection visible, labels them as stale, and blocks preparation until a retry succeeds.
2. Select one or more exams. The selection is visible on each card and carried to the handover form.
3. Choose a shift, add notes, and give every exam a specific reason. Review the shift, date, and selected folios before confirming. Check the resulting handover in `GET /api/shifthandovers`.

The app uses `GET /api/exams/pending`, `GET /api/shifts`, `POST /api/shifthandovers`, and `POST /api/auth/login`. The Hub login response supplies the authenticated user ID used for the handover. A rejected save keeps the reasons in the form for correction. If a response is lost or Hub reports a server error, the result is uncertain; the form locks submission and directs the technician to check Hub before creating another handover.

## Current limits

The app has no device interaction test in this repository. It does not yet support acknowledging another technician's handover or offline drafts. See the Hub README for server setup and sample data.
