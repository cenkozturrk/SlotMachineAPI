import requests
import time
import pandas as pd

API_URL = "https://localhost:44392/api/Player/spin/67a5338b175f6d97b8e47a78?betAmount=10"


SPIN_COUNT = 1000
results = []

for _ in range(SPIN_COUNT):
    try:
        response = requests.post(API_URL, verify=False)
        if response.status_code == 200:
            spin_result = response.json()
            results.append([    
                spin_result["winAmount"],
                spin_result["currentBalance"]
            ])
        time.sleep(0.01)
    except Exception as e:
        print(f"API error: {e}")
        break

df = pd.DataFrame(results, columns=["WinAmount", "CurrentBalance"])

print("\n 1000 Spin Test Results:")
print(f"- Totally Winning Game Count: {sum(df['WinAmount'] > 0)}")
print(f"- Avarage Gain: {df['WinAmount'].mean():.2f}")
print(f"- Minimum Gain: {df['WinAmount'].min()}")
print(f"- Maximum Gain: {df['WinAmount'].max()}")
print(f"- First Spin Balance: {df['CurrentBalance'].iloc[0]}")
print(f"- Last Spin Balance: {df['CurrentBalance'].iloc[-1]}")

df.to_csv("spin_results.csv", index=False)
print("\n The test is complete! The results are spin_results.i saved it as csv.")
