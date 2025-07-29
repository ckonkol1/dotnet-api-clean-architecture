#!/bin/bash
set -e

ENDPOINT="http://dynamodb-local:8000"
REGION="us-east-1"
TABLE_NAME="Plants"

echo "Waiting for DynamoDB Local to be ready..."
until aws dynamodb list-tables --endpoint-url $ENDPOINT --region $REGION > /dev/null 2>&1; do
    echo "DynamoDB not ready yet, waiting..."
    sleep 2
done

echo "DynamoDB Local is ready!"
echo "Attempting to Create Plants Table..."

# Create table if it does not already exist
if aws dynamodb describe-table \
    --table-name "$TABLE_NAME" \
    --region "$REGION" \
    --endpoint-url "$ENDPOINT" \
    >/dev/null 2>&1; then
    echo "Table already exists. Skipping Creation"
else
    echo "Table does not exist. Creating table..."
    if aws dynamodb create-table \
        --table-name $TABLE_NAME \
        --attribute-definitions \
            AttributeName=Id,AttributeType=S \
        --key-schema \
            AttributeName=Id,KeyType=HASH \
        --billing-mode PAY_PER_REQUEST \
        --endpoint-url $ENDPOINT \
        --region $REGION \
        >/dev/null 2>&1; then
        echo "Table created successfully"
    else
        echo "Failed to create table"
        exit 1
    fi
fi

# Wait for table to be active
echo "Waiting for $TABLE_NAME table to be active..."
aws dynamodb wait table-exists --table-name $TABLE_NAME --endpoint-url $ENDPOINT --region $REGION

# Populate with sample data
echo "Populating $TABLE_NAME table with sample data..."
aws dynamodb batch-write-item \
    --request-items file:///data/plants-seed-data.json \
    --endpoint-url $ENDPOINT \
    --region $REGION

echo "DynamoDB initialization complete!"

# List tables to verify
echo "Verifying tables:"
aws dynamodb list-tables --endpoint-url $ENDPOINT --region $REGION

# Show item count
echo "Items in $TABLE_NAME table:"
aws dynamodb scan --table-name $TABLE_NAME --select "COUNT" --endpoint-url $ENDPOINT --region $REGION