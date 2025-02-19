# dotNet5785_3252_1013
## **Oref Eitan – Volunteer Organization Management System**  

### **Project Overview**  
"Oref Eitan" is a volunteer organization that assists families affected by war. This system streamlines volunteer
management by enabling efficient task assignment and communication. Volunteers can accept, complete, or release tasks,
while administrators oversee operations and ensure timely execution.  

### **Key Features**  
- **Volunteer Management**: Volunteers can update their location and set a maximum distance for task visibility.  
- **Task Assignment**: Requests are assigned based on availability, ensuring proper tracking and completion.  
- **Risk Management**: Tasks nearing deadlines are marked as "at risk."  
- **Custom System Clock**: An internal clock, managed by the administrator, simulates time progression.  

## **User Roles and Permissions**  

### **Administrator Capabilities**  
- **Volunteer Management**: Add, update, remove, and analyze volunteer activity.  
- **Task Management**: Create, assign, update, and track tasks with full assignment history.  
- **System Clock Control**: Adjust the system clock to simulate time changes.  
- **Risk Threshold Configuration**: Define thresholds for tasks approaching deadlines.  
- **Dual Role Functionality**: Administrators can also act as volunteers.  

### **Volunteer Capabilities**  
- **Profile Management**: Update location and set a preferred task radius.  
- **Task Handling**: Accept, complete, or release a task (limited to one at a time).  
- **Task History**: View and filter past assigned tasks.  

## **Development Phases**  

### **Phase 1: Understanding C# Basics**  
Explored C# concepts such as **classes, structs, interfaces, and records**, while implementing basic data structures
and a **Data Access Layer (DAL)** with structured data contracts.  

### **Phase 2: Enhancing the DAL**  
Implemented a **generic ICrud interface**, used **LINQ** for efficient data queries, and introduced **delegates and
custom exceptions** for better modularity.  

### **Phase 3: XML Data Storage**  
Added **XML-based storage** using **LINQ to XML** and **XML serialization**, enabling data persistence beyond runtime.  

### **Phase 4: Business Logic Layer (BL) & Design Patterns**  
Introduced **Singleton** and **Factory Method** patterns, developed **BO (Business Objects)**, and implemented
structured interfaces for business operations. A **command-line test program (BlTest)** was created for manual 
verification.  

### **Phase 5: Observer Pattern & Basic UI**  
Integrated the **observer pattern** for real-time updates and developed a basic UI with entity management screens.  

### **Phase 6: Completing the UI (MVVM Pattern)**  
- Implemented **MVVM (Model-View-ViewModel)** with full **data binding** and minimal code-behind logic.  
- Improved **error handling** using user-friendly message pop-ups.  
- Used **IConverter** for dynamic UI behavior.  
- Ensured UI synchronization using the **observer pattern**.  

### **Phase 7: System Simulator**  
Developed a **simulator** that runs in parallel with the system, automating periodic updates and simulating real-world
operations. Implemented **multithreading and synchronization** to prevent race conditions.  

## **Conclusion**  
The "Oref Eitan" volunteer management system efficiently connects volunteers with families in need while ensuring task
completion, risk management, and real-time updates. By adhering to structured software development principles, we 
created a scalable and maintainable system that enhances volunteer coordination and support.

