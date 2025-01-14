import execa from 'execa';
import fse from 'fs-extra';

class FileCopy {
  src: string;
  dest: string;
  options?: fse.CopyOptions;

  constructor(filecopyOrSrc: FileCopy | string) {
    if (typeof filecopyOrSrc === 'string') {
      this.src = filecopyOrSrc;
      this.dest = filecopyOrSrc;
    } else {
      this.src = filecopyOrSrc.src;
      this.dest = filecopyOrSrc.dest;
      this.options = filecopyOrSrc.options;
    }
  }
}

const PACKAGE_TO_BUILD = 'schematics';
const FILES_TO_COPY_AFTER_BUILD: (FileCopy | string)[] = [
  { src: 'src/commands/create-lib/schema.json', dest: 'commands/create-lib/schema.json' },
  { src: 'src/commands/create-lib/files-package', dest: 'commands/create-lib/files-package' },
  {
    src: 'src/commands/create-lib/files-secondary-entrypoint',
    dest: 'commands/create-lib/files-secondary-entrypoint',
  },
  { src: 'src/commands/proxy-add/schema.json', dest: 'commands/proxy-add/schema.json' },
  { src: 'src/commands/proxy-index/schema.json', dest: 'commands/proxy-index/schema.json' },
  { src: 'src/commands/proxy-refresh/schema.json', dest: 'commands/proxy-refresh/schema.json' },
  { src: 'src/commands/proxy-remove/schema.json', dest: 'commands/proxy-remove/schema.json' },
  { src: 'src/commands/api/files-enum', dest: 'commands/api/files-enum' },
  { src: 'src/commands/api/files-model', dest: 'commands/api/files-model' },
  { src: 'src/commands/api/files-service', dest: 'commands/api/files-service' },
  { src: 'src/commands/api/schema.json', dest: 'commands/api/schema.json' },
  { src: 'src/collection.json', dest: 'collection.json' },
  'package.json',
  'README.md',
];

async function* copyPackageFile(packageName: string, filecopy: FileCopy | string) {
  filecopy = new FileCopy(filecopy);
  const { src, dest, options = { overwrite: true } } = filecopy;

  await fse.copy(
    `../packages/${packageName}/${src}`,
    `../dist/packages/${packageName}/${dest}`,
    options,
  );

  yield filecopy;
}

async function* copyPackageFiles(packageName: string) {
  for (const filecopy of FILES_TO_COPY_AFTER_BUILD) {
    yield* copyPackageFile(packageName, filecopy);
  }
}

(async () => {
  try {
    await fse.remove(`../dist/packages/${PACKAGE_TO_BUILD}`);

    await execa('yarn', ['install'], { stdout: 'inherit', cwd: `../packages/${PACKAGE_TO_BUILD}` });

    await execa(
      'tsc',
      [
        '-p',
        `packages/${PACKAGE_TO_BUILD}/tsconfig.json`,
        '--outDir',
        `dist/packages/${PACKAGE_TO_BUILD}`,
      ],
      {
        stdout: 'inherit',
        cwd: '../',
      },
    );

    for await (const filecopy of copyPackageFiles(PACKAGE_TO_BUILD)) {
      // do nothing
    }
  } catch (error) {
    process.exit(1);
  }
})();
