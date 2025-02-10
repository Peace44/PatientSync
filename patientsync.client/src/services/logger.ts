import log from 'loglevel';

// Set the log level based on the environment
if (process.env.NODE_ENV === 'development') log.setLevel('debug');
else log.setLevel('warn');

export default log;