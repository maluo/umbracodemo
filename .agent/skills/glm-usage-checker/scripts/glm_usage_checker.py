#!/usr/bin/env python3
"""
GLM Token Usage Checker
Fetches remaining quota/usage for Z.AI GLM coding plan
"""

import argparse
import json
import os
import sys
import subprocess
from datetime import datetime, timedelta
from pathlib import Path

# Try to import requests, fallback to urllib
try:
    import requests
    HAS_REQUESTS = True
except ImportError:
    HAS_REQUESTS = False

# Always import urllib for fallback
import urllib.request
import urllib.error
import ssl


def http_get(url, headers):
    """Make HTTP GET request, return (status_code, response_data)"""
    if HAS_REQUESTS:
        try:
            resp = requests.get(url, headers=headers, timeout=10)
            try:
                return resp.status_code, resp.json()
            except:
                return resp.status_code, resp.text
        except Exception as e:
            return 0, str(e)
    else:
        req = urllib.request.Request(url, headers=headers)
        # Create SSL context that bypasses cert verification (macOS compatibility)
        ctx = ssl.create_default_context()
        ctx.check_hostname = False
        ctx.verify_mode = ssl.CERT_NONE
        try:
            with urllib.request.urlopen(req, timeout=10, context=ctx) as resp:
                data = resp.read().decode('utf-8')
                try:
                    return resp.status, json.loads(data)
                except:
                    return resp.status, data
        except urllib.error.HTTPError as e:
            return e.code, e.reason
        except Exception as e:
            return 0, str(e)


def format_number(num):
    """Format number with commas"""
    return f"{num:,}"


def format_reset_time(timestamp_ms):
    """Format timestamp to human-readable relative time"""
    if not timestamp_ms:
        return "N/A"
    try:
        # Convert ms to seconds
        reset_dt = datetime.fromtimestamp(timestamp_ms / 1000)
        now = datetime.now()
        delta = reset_dt - now

        if delta.total_seconds() < 0:
            return "Now"

        hours, remainder = divmod(int(delta.total_seconds()), 3600)
        minutes = remainder // 60

        if hours > 0:
            return f"{hours}h {minutes}m"
        return f"{minutes}m"
    except:
        return "Unknown"


def get_zai_credentials():
    """Get Z.AI API key from environment variables"""
    for var in ["ZAI_API_KEY", "ZAI_KEY", "ZHIPU_API_KEY", "ZHIPUAI_API_KEY"]:
        key = os.environ.get(var)
        if key:
            return key
    return None


def get_zai_usage():
    """Fetch Z.AI usage from their monitor API"""
    api_key = get_zai_credentials()

    if not api_key:
        return {
            "error": "No credentials found",
            "hint": "Set ZAI_API_KEY environment variable",
            "dashboard": "https://z.ai/manage-apikey/billing"
        }

    result = {}
    headers = {
        "Authorization": api_key,  # Without Bearer for api.z.ai endpoints
        "Content-Type": "application/json",
    }

    # Get quota limits (the key endpoint!)
    status, data = http_get("https://api.z.ai/api/monitor/usage/quota/limit", headers)

    if status == 200 and isinstance(data, dict) and data.get("success"):
        result["status"] = "ok"
        limits = data.get("data", {}).get("limits", [])

        for limit in limits:
            limit_type = limit.get("type")
            if limit_type == "TOKENS_LIMIT":
                # Calculate total tokens based on unit (3 = 10^6 for million tokens)
                unit = limit.get("unit", 0)
                if unit == 3:
                    unit_mult = 10 ** 6  # Million tokens
                else:
                    unit_mult = 10 ** unit
                total = limit.get("number", 0) * unit_mult
                pct = limit.get("percentage", 0)
                # Calculate used and remaining from percentage
                used = int(total * pct / 100)
                remaining = total - used

                result["token_quota"] = {
                    "limit": total,
                    "used": used,
                    "remaining": remaining,
                    "percentage": pct,
                }

                # Parse reset time
                reset_ts = limit.get("nextResetTime")
                if reset_ts:
                    result["token_quota"]["resets_in"] = format_reset_time(reset_ts)

            elif limit_type == "TIME_LIMIT":
                total = limit.get("usage", 0)
                used = limit.get("currentValue", 0)
                remaining = limit.get("remaining", 0)

                result["request_quota"] = {
                    "limit": total,
                    "used": used,
                    "remaining": remaining,
                }

    # Get historical usage (last 7 days) for additional context
    now = datetime.now()
    start_date = (now - timedelta(days=7)).strftime("%Y-%m-%d+00:00:00")
    end_date = now.strftime("%Y-%m-%d+23:59:59")

    usage_url = f"https://api.z.ai/api/monitor/usage/model-usage?startTime={start_date}&endTime={end_date}"
    status, data = http_get(usage_url, headers)

    if status == 200 and isinstance(data, dict) and data.get("success"):
        usage_data = data.get("data", {})
        total = usage_data.get("totalUsage", {})

        if total:
            if "status" not in result:
                result["status"] = "ok"
            result["weekly_usage"] = {
                "calls": total.get("totalModelCallCount", 0),
                "tokens": total.get("totalTokensUsage", 0),
            }

    # Fallback: get user info if main APIs failed
    if "status" not in result:
        auth_headers = {"Authorization": f"Bearer {api_key}", "Content-Type": "application/json"}
        status, data = http_get("https://chat.z.ai/api/v1/auths/", auth_headers)
        if status == 200:
            result["status"] = "authenticated"

    # Add hints
    result["dashboard"] = "https://z.ai/manage-apikey/billing"

    return result


def generate_table_output(result):
    """Generate formatted table output for display"""
    lines = []
    lines.append("GLM Token Usage Check")
    lines.append("=" * 50)

    if "error" in result:
        lines.append(f"\n[X] {result['error']}")
        if "hint" in result:
            lines.append(f"[!] {result['hint']}")
        if "dashboard" in result:
            lines.append(f"[->] Dashboard: {result['dashboard']}")
        return "\n".join(lines)

    # Show status
    status = result.get("status")
    if status == "ok":
        lines.append("\n[OK] Connected")
    elif status == "authenticated":
        lines.append("\n[OK] Authenticated")
    else:
        lines.append(f"\n[?] Status: {status}")

    # Show token quota (5-hour window)
    if "token_quota" in result:
        tq = result["token_quota"]
        lines.append("\n[*] Token Quota (5-hour window):")
        lines.append(f"  Total:      {format_number(tq['limit'])} tokens")
        lines.append(f"  Used:       {format_number(tq['used'])} tokens ({tq['percentage']:.1f}%)")
        lines.append(f"  Remaining:  {format_number(tq['remaining'])} tokens ({100 - tq['percentage']:.1f}%)")
        if "resets_in" in tq:
            lines.append(f"  Resets in:  {tq['resets_in']}")

    # Show request quota
    if "request_quota" in result:
        rq = result["request_quota"]
        lines.append("\n[*] Request Quota:")
        lines.append(f"  Total:      {format_number(rq['limit'])}")
        lines.append(f"  Used:       {format_number(rq['used'])}")
        lines.append(f"  Remaining:  {format_number(rq['remaining'])}")

    # Show weekly usage
    if "weekly_usage" in result:
        wu = result["weekly_usage"]
        lines.append("\n[*] Weekly Usage (last 7 days):")
        lines.append(f"  Total API calls:  {format_number(wu['calls'])}")
        lines.append(f"  Total tokens:     {format_number(wu['tokens'])}")

    # Show dashboard link
    if "dashboard" in result:
        lines.append(f"\n[->] Dashboard: {result['dashboard']}")

    lines.append("\n")
    return "\n".join(lines)


def print_usage():
    """Print GLM usage information"""
    result = get_zai_usage()
    print(generate_table_output(result))


def open_in_psmux(content):
    """Open content in a new psmux split pane"""
    # Create a temporary file with the content
    import tempfile
    with tempfile.NamedTemporaryFile(mode='w', suffix='.txt', delete=False, encoding='utf-8') as f:
        f.write(content)
        temp_file = f.name

    try:
        # psmux uses tmux-compatible commands
        # split-window -h = horizontal split (side by side)
        # split-window -v = vertical split (top/bottom)
        # On Windows, use 'type' instead of 'cat', and PowerShell for pause
        cmd = f'type {temp_file} & pause'
        subprocess.run(['psmux', 'split-window', '-h', cmd], shell=True)
        print("\n[+] Opened in psmux split pane")
    except Exception as e:
        print(f"[!] Failed to open in psmux: {e}")
        print("\n" + content)
    finally:
        # Clean up temp file after a delay
        try:
            import threading
            def cleanup():
                import time
                time.sleep(60)  # Give more time to read
                try:
                    os.unlink(temp_file)
                except:
                    pass
            threading.Thread(target=cleanup, daemon=True).start()
        except:
            pass


def main():
    parser = argparse.ArgumentParser(
        description='Check GLM (Z.AI) token usage and quota',
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
  python glm_usage_checker.py              # Show usage in terminal
  python glm_usage_checker.py --split      # Open in psmux split pane
  python glm_usage_checker.py -s           # Short form for --split
        """
    )
    parser.add_argument('-s', '--split', action='store_true',
                        help='Display output in a psmux split pane')
    parser.add_argument('-w', '--watch', action='store_true',
                        help='Watch mode - refresh every 30 seconds (requires --split)')

    args = parser.parse_args()

    if args.split:
        if args.watch:
            # Watch mode - continuously refresh
            print("[*] Starting watch mode (Ctrl+C to stop)...")
            import time
            try:
                while True:
                    result = get_zai_usage()
                    content = generate_table_output(result)
                    content = f"GLM Usage Watch - {datetime.now().strftime('%H:%M:%S')}\n" + content
                    open_in_psmux(content)
                    time.sleep(30)
            except KeyboardInterrupt:
                print("\n[!] Watch mode stopped")
        else:
            # Single display in split pane
            result = get_zai_usage()
            content = generate_table_output(result)
            open_in_psmux(content)
    else:
        # Normal terminal output
        print_usage()


if __name__ == "__main__":
    main()
