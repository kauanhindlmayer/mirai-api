# Aspire Troubleshooting Guide

This guide provides troubleshooting steps for common issues encountered while using Aspire.

## Dashboard Port Already in Use

When starting the Aspire app, you may encounter the following error:

```bash
System.IO.IOException: Failed to bind to address https://127.0.0.1:17156: address already in use.
```

This usually happens when a previous instance of the dashboard didn’t shut down cleanly.

### Solution

1. Identify the process using the port:

```bash
lsof -i :17156
```

2. Kill the process:

```bash
kill <PID>
```
