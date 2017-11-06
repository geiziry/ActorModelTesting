;;; Tune the GC
;; The default settings are too conservative on modern machines making Emacs
;; spend too much time collecting garbage in alloc-heavy code.
(setq gc-cons-threshold (* 4 1024 1024))
(setq gc-cons-percentage 0.3)

;;; Initialize the package manager
(eval-and-compile
  (require 'package)
  (setq package-archives '(("melpa" . "http://melpa.org/packages/")
                           ("gnu" . "http://elpa.gnu.org/packages/")))
  (setq package-enable-at-startup nil)
  (package-initialize)
  (defvar init-el-package-archives-refreshed nil)
  (defun init-el-install-package (package-name)
    (unless (package-installed-p package-name)
      (unless init-el-package-archives-refreshed
        (package-refresh-contents)
        (setq init-el-package-archives-refreshed t))
      (package-install package-name)))
  (defmacro init-el-with-eval-after-load (feature &rest body)
    (declare (indent 1) (debug t))
    (require feature)
    `(with-eval-after-load ',feature ,@body))
  (defmacro init-el-require-package (package-name &optional feature-name)
    (init-el-install-package package-name)
    (require (or feature-name package-name))
    `(init-el-install-package ',package-name)))

;;; Set window size
(setq default-frame-alist '((width . 130) (height . 45)))

;;;Show line number
(global-linum-mode t)

;;; Disable useless GUI stuff
(tool-bar-mode -1)
(scroll-bar-mode -1)
(when (fboundp 'horizontal-scroll-bar-mode)
  (horizontal-scroll-bar-mode -1))
(menu-bar-mode -1)
(blink-cursor-mode -1)
(setq use-file-dialog nil)
(setq use-dialog-box nil)

;;; Start with empty scratch buffer
(fset #'display-startup-echo-area-message #'ignore)
(setq inhibit-splash-screen t)
(setq initial-scratch-message "")

;;; Set the font
(set-face-attribute 'default nil :font "Ubuntu Mono" :height 130)

;;; Disable lock files
(setq create-lockfiles nil)

;;; Disable backup files
(setq make-backup-files nil)

;;; Move auto-save files to saner location
(let ((auto-save-dir (file-name-as-directory (expand-file-name "autosave" user-emacs-directory))))
  (setq auto-save-list-file-prefix (expand-file-name ".saves-" auto-save-dir))
  (setq auto-save-file-name-transforms (list (list ".*" (replace-quote auto-save-dir) t))))

;;; Use UTF-8
(prefer-coding-system 'utf-8)
(set-language-environment "UTF-8")
(setq locale-coding-system 'utf-8)
(set-selection-coding-system 'utf-8)
(setq-default buffer-file-coding-system 'utf-8-unix)

;;; Fix scrolling
(setq mouse-wheel-progressive-speed nil)
(setq scroll-margin 3)
(setq scroll-conservatively 100000)
(setq scroll-preserve-screen-position 'always)

;;; Clipboard
(setq-default select-active-regions nil)
(when (boundp 'x-select-enable-primary)
  (setq x-select-enable-primary nil))

;;; Set undo limits
(setq undo-limit (* 16 1024 1024))
(setq undo-strong-limit (* 24 1024 1024))
(setq undo-outer-limit (* 64 1024 1024))

;;; Do not disable commands
(setq disabled-command-function nil)

;;; Disable electrict indent
(when (bound-and-true-p electric-indent-mode)
  (electric-indent-mode -1))

;;; undo-tree
(init-el-require-package undo-tree)
(global-undo-tree-mode)
(setq undo-tree-visualizer-timestamps t)
(setq undo-tree-visualizer-lazy-drawing nil)
(setq undo-tree-auto-save-history t)
(let ((undo-dir (expand-file-name "undo" user-emacs-directory)))
  (setq undo-tree-history-directory-alist (list (cons "." undo-dir))))

;;; Ignore case for completion
(setq completion-ignore-case t)
(setq read-buffer-completion-ignore-case t)
(setq read-file-name-completion-ignore-case t)

;;; History saving
(require 'savehist)
(setq history-length 1024)
(setq search-ring-max 1024)
(setq regexp-search-ring-max 1024)
(setq savehist-additional-variables '(extended-command-history file-name-history search-ring regexp-search-ring))
(setq savehist-file (expand-file-name ".savehist" user-emacs-directory))
(savehist-mode)

(add-to-list 'load-path "~/.emacs.d/custom")

(require 'setup-general)
    (require 'setup-ivy-counsel)
  (require 'setup-helm)
  (require 'setup-helm-gtags)
;; (require 'setup-ggtags)
(require 'setup-cedet)
(require 'setup-editing)


;;; show-paren-mode
(require 'paren)
(show-paren-mode)
(setq show-paren-delay 0)

;;; Set the theme
(init-el-require-package color-theme-sanityinc-tomorrow emacs)
(defconst init-el-default-theme 'sanityinc-tomorrow-eighties)
(load-theme init-el-default-theme t)
(add-hook 'after-make-frame-functions #'init-el-enable-theme)
(deftheme init-el-overrides)
(cl-macrolet
    ((set-face
      (face &rest attributes)
      `'(,face ((((class color) (min-colors 89)) (,@attributes))))))
  (custom-theme-set-faces
   'init-el-overrides
   (set-face show-paren-match :foreground nil :background nil :underline "#66cccc")
   (set-face show-paren-mismatch :foreground nil :background nil :underline "#f2777a")
   (set-face rainbow-delimiters-depth-1-face :foreground "#e67c7c")
   (set-face rainbow-delimiters-depth-2-face :foreground "#cf9d9d")
   (set-face rainbow-delimiters-depth-3-face :foreground "#edb082")
   (set-face rainbow-delimiters-depth-4-face :foreground "#d4d484")
   (set-face rainbow-delimiters-depth-5-face :foreground "#a0cca0")
   (set-face rainbow-delimiters-depth-6-face :foreground "#b3cc8b")
   (set-face rainbow-delimiters-depth-7-face :foreground "#9f9fdf")
   (set-face rainbow-delimiters-depth-8-face :foreground "#88aabb")
   (set-face rainbow-delimiters-depth-9-face :foreground "#c08ad6")))

(defun init-el-enable-theme (_frame)
  (enable-theme init-el-default-theme)
  (enable-theme 'init-el-overrides)
  (remove-hook 'after-make-frame-functions #'init-el-enable-theme))



;;; rainbow-delimiters
(init-el-require-package rainbow-delimiters)
(add-hook 'lisp-mode-hook #'rainbow-delimiters-mode)
(add-hook 'emacs-lisp-mode-hook #'rainbow-delimiters-mode)
(add-hook 'scheme-mode-hook #'rainbow-delimiters-mode)


;;; smartparens
(init-el-require-package smartparens)
(require 'smartparens-config)
(smartparens-global-mode)
(setq sp-highlight-pair-overlay nil)
(setq sp-highlight-wrap-overlay nil)
(setq sp-highlight-wrap-tag-overlay nil)
(setq-default sp-autoskip-closing-pair t)
(sp-local-pair '(c-mode c++-mode java-mode css-mode php-mode js-mode perl-mode
                        cperl-mode rust-mode sh-mode)
               "{" nil
               :post-handlers '((init-el-smartparens-create-and-enter-block "RET")))

(defun init-el-smartparens-create-and-enter-block (&rest _)
  (save-excursion
    ;; Indent the line with the opening brace, but only if it
    ;; contains nothing more than the brace.
    (end-of-line 0)
    (let ((old-point (point)))
      (back-to-indentation)
      (when (= (1+ (point)) old-point)
        (indent-according-to-mode))))
  ;; Open the block and reindent the closing brace.
  (newline)
  (indent-according-to-mode)
  ;; Enter it.
  (forward-line -1)
  (indent-according-to-mode))

;;; avy
(init-el-require-package avy)
(init-el-with-eval-after-load avy
  (setq avy-style 'pre)
  (setq avy-keys (eval-when-compile (number-sequence ?a ?z)))
  (setq avy-all-windows nil)
  (setq avy-case-fold-search nil))


;;; windmove
(require 'windmove)
(setq windmove-wrap-around t)


;;; Irony
;;(init-el-require-package irony)
(add-hook 'c++-mode-hook 'irony-mode)
(add-hook 'c-mode-hook 'irony-mode)
;; (defun my-irony-mode-hook()
;;   (define-key irony-mode-map [remap completion-at-point]
;;   'irony-completion-at-point-async)
;;   (define-key irony-mode-map[remap complete-symbol]
;;   'irony-completion-at-point-async))
;; (eval-after-load 'company
;;   '(add-to-list 'company-backends 'company-irony))
;; (add-hook 'irony-mode-hook 'my-irony-mode-hook)
(add-hook 'irony-mode-hook 'irony-cdb-autosetup-compile-options)
(add-hook 'irony-mode-hook 'company-irony-setup-begin-commands)
(setq company-backends(delete 'company-semantic company-backends))

(setq company-idle-delay 0)
(define-key c-mode-map [(tab)] 'company-complete)
(define-key c++-mode-map [(tab)] 'company-complete)
;;company-irony-c-headers
(require 'company-irony-c-headers)
(with-eval-after-load 'company
  '(add-to-list
    'comany-backends '(company-irony-c-headers company-irony))
  )


;;flycheck
(add-hook 'c++-mode-hook 'flycheck-mode)
(add-hook 'c-mode-hook 'flycheck)
(setq irony-additional-clang-options '("-std=c++11"))

;;yasnippet
;; (require 'yasnippet)
;; (yas-global-mode 1)

(use-package avy
  :ensure t
  :bind ("M-s" . avy-goto-char))
;; function-args
;; (require 'function-args)
;; (fa-config-default)
;; (define-key c-mode-map  [(tab)] 'company-complete)
;; (define-key c++-mode-map  [(tab)] 'company-complete)
(custom-set-variables
 ;; custom-set-variables was added by Custom.
 ;; If you edit it by hand, you could mess it up, so be careful.
 ;; Your init file should contain only one such instance.
 ;; If there is more than one, they won't work right.
 '(package-selected-packages
   (quote
    (async avy rainbow-delimiters smartparens sr-speedbar swiper projectile emmet-mode flycheck-irony function-args helm-core helm-projectile helm-swoop highlight-quoted ivy company-irony company-irony-c-headers counsel irony zygospore helm-gtags helm yasnippet ws-butler volatile-highlights use-package undo-tree iedit dtrt-indent counsel-projectile company clean-aindent-mode anzu)))
 '(safe-local-variable-values (quote ((flycheck-clang-language-standard . c++11)))))
(custom-set-faces
 ;; custom-set-faces was added by Custom.
 ;; If you edit it by hand, you could mess it up, so be careful.
 ;; Your init file should contain only one such instance.
 ;; If there is more than one, they won't work right.
 )
