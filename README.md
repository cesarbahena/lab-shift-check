# ShiftCheck

Mobile application for shift handover management in laboratory environments.

## Overview

ShiftCheck is a .NET MAUI mobile application designed for managing shift handovers in laboratory settings. It allows laboratory staff to track pending samples and create accountability reports during shift transitions.

## Features

- User authentication
- View pending laboratory samples
- Select samples for shift handover
- Create shift handover reports with notes
- Track accountability for pending results

## Technology Stack

- .NET 6.0
- .NET MAUI (Multi-platform App UI)
- CommunityToolkit.Mvvm
- HttpClient for API communication

## Dependencies

This application depends on QuimiOSHub API for backend services.

## Platforms

- Android 21.0+
- iOS 14.2+

## Architecture

The application follows the MVVM pattern with:
- Models: Data transfer objects matching QuimiOSHub API
- ViewModels: Business logic and state management
- Views: XAML-based UI pages
- Services: API communication and authentication

## Configuration

The API base URL is configured in `MauiProgram.cs`. Update the HttpClient configuration to point to your QuimiOSHub instance.

## Getting Started

1. Ensure QuimiOSHub is running
2. Build and run the ShiftCheck application
3. Login with your credentials
4. View pending samples and create shift handovers
