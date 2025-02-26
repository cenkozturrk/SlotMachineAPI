# Slot Machine API - MongoDB Installation

### Running MongoDB on the Local Machine
1. **Make sure that MongoDB Compass** is installed.
2. **Connect with MongoDB Compass**  
   - Connection address: `mongodb://localhost:27017`
   - Database name: `SlotMachineDB`
3. **If you are using Docker** you can start MongoDB by running the following command:
   ```bash
   docker run -d --name mongodb-container -p 27017:27017 mongo
