# Agile Board

This repository contains only the timetracker mobile app codebase
Please see other repositories to find all system apps (see tech stack section)

## About AgileBoard
AgileBoard is an innovative, open-source project management web application designed to streamline the complexities of <b>project and task management</b> for teams of any size. At its core, AgileBoard offers a dynamic platform for creating, organizing, and tracking tickets across various milestones or sprints, enabling teams to focus on what matters most - delivering value.

With the Agile board, teams can visualize their active sprints in a fully customizable interface, adapting the board to fit the unique workflow and processes of each project. This visual representation not only simplifies project oversight but also enhances team collaboration and productivity by providing a clear overview of task progress and dependencies.

AgileBoard goes beyond traditional project management tools by incorporating a Knowledge Base module. This feature allows teams to curate and access vital project-related articles, documents, and information, ensuring that valuable insights and resources are always within reach.

Understanding the importance of efficient resource management, AgileBoard includes a comprehensive calendar module. This tool assists in scheduling, planning, and managing team members, facilitating optimal allocation of human resources across tasks and projects.

The latest addition to AgileBoard's suite of features is the <b>time tracking</b> capability. Users can now effortlessly record their work hours manually or utilize the convenience of our computer and mobile applications for automatic time logging. This feature not only enhances project billing and accounting practices but also provides invaluable data for analyzing productivity and optimizing workflows.

AgileBoard is more than a project management tool; it's a companion in your journey towards agile excellence. By embracing AgileBoard, teams can harness the power of agility, collaboration, and information to drive project success and achieve their goals.

#### AgileBoard tech stack:
- backend is based on Laravel framwework [backend repository](https://github.com/fenzelek/agileboard_backend.git)
- frontend: AngularJS  [frontend repository](https://github.com/fenzelek/agileboard_frontend.git)
- time tracking app: Angular electron [timetracker PC repository](https://github.com/fenzelek/agileboard_timetracker_pc.git)
- time tracking mobile app : Xamarin [https://github.com/fenzelek/agileboard_timetracker_mobile.git]

### SPONSORS
### **[Denkkraft](https://denkkraft.eu/)**

### Security Vulnerabilities
If you discover a security vulnerability within Laravel, please send an e-mail to [opensource@denkkraft.eu](mailto:opensource@denkkraft.eu) . All security vulnerabilities will be promptly addressed.

# License
The Agile Board is open-sourced software licensed under the [MIT license](https://opensource.org/licenses/MIT)

# HOW TO RUN

# TimeTrackerXamarin

Time tracker allows AgileBoard mobile users to track their time.

## Testing and building

There is Dockerfile that allows testing and building project.

First, docker image has to be built using:

```docker build -t time_tracker_builder -f docker/Dockerfile .```

Then it can be used to:

### Run tests

```docker run --name time_tracker_builder --rm -it time_tracker_builder test```

### Build Android project

```docker run --name time_tracker_builder --rm -v <output directory>:/output/ -it time_tracker_builder build-android```

/output/ directory must be mounted, project will not build otherwise.

Currently only **DEV** version can be built.