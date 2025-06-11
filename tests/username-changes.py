import os
import time
import requests
import psycopg2
import urllib3

urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

# Define constants
USER_ID = "b3f8a81d-5835-49f2-a643-feacbc996f3b"
TEST_USERNAME = "halloDitIsEenTestGebruiker123@"
RESET_USERNAME = "default123"
ACCOUNT_API_URL = f"https://131.189.152.107.nip.io/api/accounts/{USER_ID}/name"

# Environment variables/secrets
ACCOUNT_DB_CONNECTION_STRING = os.getenv("ACCOUNT_DB_CONNECTION_STRING", "")
POSTS_DB_CONNECTION_STRING = os.getenv("POSTS_DB_CONNECTION_STRING", "")
POST_SEARCH_DB_CONNECTION_STRING = os.getenv("POST_SEARCH_DB_CONNECTION_STRING", "")

print("Running integration test for PATCH request to update username...")

# 1. Send PATCH request to update username
response = requests.patch(
    ACCOUNT_API_URL,
    data=f'"{TEST_USERNAME}"',
    headers={"Content-Type": "application/json"},
    verify=False,
)
assert response.status_code in (200, 204), (
    f"PATCH failed: {response.status_code} {response.text}"
)

print(f"Called {ACCOUNT_API_URL} with '{TEST_USERNAME}")
print("Waiting for data changes, sleeping 5s...")

# 2. Wait for eventual consistency
time.sleep(5)  # Adjust as needed

print("Validating changes...")

# 3. Connect to AccountsDb and verify user exists
accounts_db_conn = psycopg2.connect(ACCOUNT_DB_CONNECTION_STRING)
with accounts_db_conn.cursor() as cursor:
    cursor.execute('SELECT 1 FROM "Accounts" WHERE "Id" = %s', (USER_ID,))
    assert cursor.fetchone(), "User not found in AccountsDb"

# 4. Connect to PostsDb and verify post author username updated
posts_db_conn = psycopg2.connect(POSTS_DB_CONNECTION_STRING)
with posts_db_conn.cursor() as cursor:
    cursor.execute(
        'SELECT 1 FROM "Posts" WHERE "AuthorId" = %s AND "AuthorName" = %s',
        (USER_ID, TEST_USERNAME),
    )
    assert cursor.fetchone(), "No post found with updated username in PostsDb"

# 5. Connect to PostsSearchDb and verify post author username updated
posts_db_conn = psycopg2.connect(POST_SEARCH_DB_CONNECTION_STRING)
with posts_db_conn.cursor() as cursor:
    cursor.execute(
        'SELECT 1 FROM "Posts" WHERE "AuthorId" = %s AND "AuthorName" = %s',
        (USER_ID, TEST_USERNAME),
    )
    assert cursor.fetchone(), "No post found with updated username in PostSearchDB"

print("Changes validated successfully.")
print("Integration test completed successfully!")

RESET_USERNAME = "Test123"
reset_response = requests.patch(
    ACCOUNT_API_URL,
    data=f'"{RESET_USERNAME}"',
    headers={"Content-Type": "application/json"},
    verify=False,
)
assert reset_response.status_code in (200, 204), (
    f"Reset PATCH failed: {reset_response.status_code} {reset_response.text}"
)

print(f"Username reset to default value '{RESET_USERNAME}' successfully!")
print("All tests passed successfully!")
